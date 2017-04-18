#include <iostream>
#include <string>
#include <ctime>
#include <exception>
#include <conio.h>
#include <windows.h>

/*
#include <SFML/Audio.hpp>
#include <SFML/Graphics.hpp>
*/

#include "AgentMode.h"
#include "GameTime.h"
#include "Mobj.h"
#include "SysFunc.h"
#include "Recorddef.h"

/*
TODO LIST:

- Test multiple enemies. they sometimes overlap, need a random AI option when they overlap and need a way to detect this overlap
- Add high score system. If stop playing by whatever means, add to highscore list
- Add more map elements/mobjs

*/

Map*		CurrentMap = nullptr;

using namespace std;

//int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE, PWSTR pCmdLine, int nCmdShow)
int main() {
	GameMode gamemode;
	SetConsoleTitle("Hunted");
	int levelcounter = 0;
	clock_t t_i, t_f, t_temp;
	bool agentmode = false;
	do {

	GameLoop:

		ShowConsoleCursor(false);
		DisplayMenu();

		gamemode = AskGameMode();

		clearscreen();

		if (gamemode == GM_NORMAGENT) {
			g_delay(0.075); // have delay between inputs in order to avoid instant screen switches
			DisplayAgentMode();
			agentmode = AskAgentMode();
			clearscreen();
		}

		g_delay(0.075); // have delay between inputs in order to avoid instant screen switches
		DisplayDifficulty();
		
		Difficulty diff = AskDifficulty();
		clearscreen();
		g_delay(0.075);

		GameState gstate = GS_PLAYING;
		Map* CurrMap = GenerateMap(levelcounter, gamemode);
		CurrentMap = CurrMap;
		Player* p = GeneratePlayer(CurrMap);
		Agent* agent = nullptr;
		if (gamemode == GM_NORMAGENT) {
			Entity^ entity = gcnew Entity("HuntedAgent");
			entity->setLearningSystemType(LearningSystemType::LS_QLEARNING);
			agent = new Agent(p, entity);
			// agentmode is either train or use recorded data, true = train
			agent->setLearningSystem(agentmode);
			if (agentmode) {
				agent->getLearningSystem()->setLogging(false);
				agent->getLearningSystem()->setIterations(300);
				agent->getLearningSystem()->setGamma(0.5);
				SendRMatrix(agent);
				if (agent->getLearningSystem()->train_randomgoals()) {
					cout << "Training complete!\n Reverting back to the menu...";
					agent->getEntity()->exportLearnedData();
					g_delay(1.0);
					clearscreen();
					g_delay(0.075);
					goto GameLoop;
				}
			}
		}
		CurrentMap->show_map();
		while (gstate != GS_LOST && gstate != GS_EXIT) {
			// Player starts
			p = GeneratePlayer(CurrMap);
			p->draw();

			// Create the object and assign path finder
			int frame = 0;
			EnemyList M;
			Setup_Enemylist(M, CurrMap);

			IssuePathSearch(M, p, CurrMap);

			t_i = clock();

			// real game loop
			while (gstate == GS_PLAYING) {
				t_temp = clock();
				p->cleanprevframe();
				p->set_prevpos(p->getpos().x, p->getpos().y); // previous place of player is now empty

				if (gamemode != GM_NORMAGENT) // take input from user only if it's not agent playing
					ListenInput(p, CurrMap);
				else
					AgentTakeInput(agent);

				if (p->hasmoved()) // if not same as before
					IssuePathSearch(M, p, CurrMap);

				p->draw();

				CleanPreviousEnemyFrame(M, CurrMap); // prevents the old enemy frames from being visible on the map
				UpdateFrames(M, frame, diff); // update the position of the enemies
				DrawEnemies(M); // and show them to us

				if (PlayerFound(M, p)) { // if any enemy caught up with player
					gstate = GS_LOST;
					break; // end the game
				}

				if (CheckWinCondition(p, CurrMap)) {
					t_f = clock();
					GameTime endtime(t_f - t_temp); // spent time on the map
					bool exit = DisplayWonMessage(p, endtime);
					if (exit) {
						gstate = GS_EXIT;
						break;
					}
					else {
						delete CurrMap; // don't delete if we exit, it will try to delete a null ptr at the bottom statements
						// Reset player score, save it to total points and reset the map
						gstate = GS_PLAYING;
						p->addtotalpts(p->getpts());
						p->set_pts(0);
						p->add_finish_time(endtime.get_time());
						CurrMap = GenerateMap(++levelcounter, gamemode);
						if (levelcounter == GetMaxLevelsAllowed(gamemode)) // reached end of maps allowed
							gstate = GS_EXIT;
						break; // don't execute below statements, just go back to first loop
					}
				}

				ShowInfo(p, CurrMap, t_i);

				GameTics(frame);
			}
		}

		clearscreen();
		ShowConsoleCursor(true);
		levelcounter = 0;
		t_f = clock();
		GameTime endtime(t_f - t_i); // overall spent time on the game
		DisplayEndScreen(p, gstate, endtime);
		g_delay(0.075);
		delete agent;
		delete CurrMap;
		delete p;
	} while (!PromptExit());
	return 0;
}