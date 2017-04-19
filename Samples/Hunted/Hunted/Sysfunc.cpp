#include <conio.h>
#include <ctime>

#include "GameFuncs.h"
#include "SysFunc.h"
#include "Mobj.h"
#include "Maplist.h"

using namespace std;

#if defined(WIN32) || defined(_WIN32) || defined(__WIN32) && !defined(__CYGWIN__)
void clearscreen() {
	system("cls");
}
#else
void clearscreen() {
	// CSI[2J clears screen, CSI[H moves the cursor to top-left corner
	std::cout << "\x1B[2J\x1B[H";
}
#endif

// OPERATOR OVERLOADS

ostream& operator<< (ostream &stm, const COLOR &c) {
	HANDLE out_handle = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(out_handle, c);
	return stm;
}

string operator+ (string s1, string s2) {
	string s3 = "";
	s3.append(s1);
	s3.append(s2);
	return s3;
}

void operator|= (PState &S, PState T) {
	S = PState(S | T);
}
void operator^= (PState &S, PState T) {
	S = PState(S ^ T);
}

// END OF OVERLOADS

template <typename T>
string to_string(T x) {
	stringstream ss;
	ss << x;
	return ss.str();
}

void DisplayMenu() {
	cout << white << "Instructions:\n1. Arrow keys to move your hero\n2. Eat the scores (symbols) to finish maps\n3. Don't get caught by the Enemy\n\nChoose the game mode\n\n" << normal << "   Normal Mode\n   Normal AI Agent Mode\n   Survival Mode\n   Collector Mode";
}

void DisplayDifficulty() {
	cout << "Choose the difficulty\n\n" << lightgreen << "   Easy\n" << yellow << "   Normal\n" << lightred << "   Hard\n" << normal;
}

void DisplayAgentMode() {
	cout << "Choose the agent mode\n\n" << lightgreen << "   Training\n" << yellow << "   Play\n" << normal;
}

void DisplayMapSelect() {
	cout << "Choose the map to play\n\n" << lightgreen << "   1. Level 1\n   2. Level 2\n   3. Level 3\n" << normal;
}

void drawatxy(int x, int y, char c) {
	gotoxy(x, y);
	cout << c;
}

GameMode AskGameMode() {
	// xy norm = 1, 7 y++ for rest
	point base(1, 7);
	drawatxy(1, 7, '*');
	while (true) {
		if (GetAsyncKeyState(VK_RETURN) & 0x8000) { // is this key down at the moment this function is called?
			switch (base.y) {
			case 7:
				return GM_NORM;
			case 8:
				return GM_NORMAGENT;
			case 9:
				return GM_SURVIVAL;
			case 10:
				return GM_COLLECTOR;
			}
		}
		else if (GetAsyncKeyState(VK_DOWN) & 0x8000) {
			if (base.y < 10) {
				drawatxy(1, base.y, ' '); // clean prev frame
				base.y++;
				drawatxy(1, base.y, '*'); // update new frame
				g_delay(0.125);
			}
		}
		else if (GetAsyncKeyState(VK_UP) & 0x8000) {
			if (base.y > 7) {
				drawatxy(1, base.y, ' ');
				base.y--;
				drawatxy(1, base.y, '*');
				g_delay(0.125);
			}
		}
		else
			_getch();
	}
}

Difficulty AskDifficulty() {
	// xy norm = 1, 2 y++ for rest
	point base(1, 2);
	drawatxy(1, 2, '*');
	while (GetAsyncKeyState(VK_RETURN)) // while it is being pressed, do not consider any input until we let go of the key
		g_delay(0.001);
	while (true) { // now we let go of it, consider inputs once more
		if (GetAsyncKeyState(VK_RETURN) & 0x8000) {
			switch (base.y) {
			case 2:
				return DIFF_EASY;
			case 3:
				return DIFF_NORM;
			case 4:
				return DIFF_HARD;
			default:
				return DIFF_INVALID;
			}
		}
		else if (GetAsyncKeyState(VK_DOWN) & 0x8000) {
			if (base.y < 4) {
				drawatxy(1, base.y, ' ');
				base.y++;
				drawatxy(1, base.y, '*');
				g_delay(0.125);
			}
		}
		else if (GetAsyncKeyState(VK_UP) & 0x8000) {
			if (base.y > 2) {
				drawatxy(1, base.y, ' ');
				base.y--;
				drawatxy(1, base.y, '*');
				g_delay(0.125);
			}
		}
		else
			_getch();
	}
}

bool AskAgentMode() {
	// xy norm = 1, 2 y++ for rest
	point base(1, 2);
	drawatxy(1, 2, '*');
	while (GetAsyncKeyState(VK_RETURN)) // while it is being pressed, do not consider any input until we let go of the key
		g_delay(0.001);
	while (true) { // now we let go of it, consider inputs once more
		if (GetAsyncKeyState(VK_RETURN) & 0x8000) {
			switch (base.y) {
			case 2:
				return true;
			case 3:
				return false;
			default:
				return false;
			}
		}
		else if (GetAsyncKeyState(VK_DOWN) & 0x8000) {
			if (base.y < 3) {
				drawatxy(1, base.y, ' ');
				base.y++;
				drawatxy(1, base.y, '*');
				g_delay(0.125);
			}
		}
		else if (GetAsyncKeyState(VK_UP) & 0x8000) {
			if (base.y > 2) {
				drawatxy(1, base.y, ' ');
				base.y--;
				drawatxy(1, base.y, '*');
				g_delay(0.125);
			}
		}
		else
			_getch();
	}
}

int AskMapNumber() {
	// xy norm = 1, 2 y++ for rest
	point base(1, 2);
	drawatxy(1, 2, '*');
	while (GetAsyncKeyState(VK_RETURN)) // while it is being pressed, do not consider any input until we let go of the key
		g_delay(0.001);
	while (true) { // now we let go of it, consider inputs once more
		if (GetAsyncKeyState(VK_RETURN) & 0x8000) {
			return base.y - 2;
		}
		else if (GetAsyncKeyState(VK_DOWN) & 0x8000) {
			if (base.y < 4) {
				drawatxy(1, base.y, ' ');
				base.y++;
				drawatxy(1, base.y, '*');
				g_delay(0.125);
			}
		}
		else if (GetAsyncKeyState(VK_UP) & 0x8000) {
			if (base.y > 2) {
				drawatxy(1, base.y, ' ');
				base.y--;
				drawatxy(1, base.y, '*');
				g_delay(0.125);
			}
		}
		else
			_getch();
	}
}

Map* GenerateMap(int levelnum, GameMode gm) {
	const point p = GetLevelDim(levelnum, gm);
	Map* CurrMap = new Map(p.x, p.y);
	CurrMap->load_map(levelnum, gm);
	CurrMap->set_gamemode(gm);
	CurrMap->set_levelcount(levelnum);
	return CurrMap;
}

Player* GeneratePlayer(Map* CurrMap) {
	return new Player(CurrMap->get_start().x, CurrMap->get_start().y);
}

void gotoxy(short x, short y) {
	HANDLE hStdout = GetStdHandle(STD_OUTPUT_HANDLE);
	COORD position = { x, y };

	SetConsoleCursorPosition(hStdout, position);
}

template <typename T>
void g_delay(T sec) {
	// The reference
	HANDLE hConsole = GetStdHandle(STD_INPUT_HANDLE);
	clock_t start_time = clock();
	clock_t end_time = clock_t(sec * 1000);
	// Explanation: X - X_0 = Y. X is incremented by 1 ms, X_0 is a constant of current time. Y is destination for our time.
	while (clock() - start_time < end_time)
		// this is necessary, otherwise the player will be able to interrupt the countdown
		FlushConsoleInputBuffer(hConsole);
}

//template void g_delay < int >(int);
//template void g_delay < double >(double);

bool WaitKeyPress(unsigned int timeout_ms = 0) {
	return WaitForSingleObject(GetStdHandle(STD_INPUT_HANDLE), timeout_ms) == WAIT_OBJECT_0;
}

bool Request_Key_Press(int fail, int succeed) {
	while (true) {
		if (GetAsyncKeyState(fail))
			return true;
		else if (GetAsyncKeyState(succeed)) {
			clearscreen();
			return false;
		}
		else
			_getch(); // ignore unused characters without showing them
	}
}

void ShowConsoleCursor(bool showFlag) {
	HANDLE out = GetStdHandle(STD_OUTPUT_HANDLE);

	CONSOLE_CURSOR_INFO     cursorInfo;

	GetConsoleCursorInfo(out, &cursorInfo);
	cursorInfo.bVisible = showFlag; // set the cursor visibility
	SetConsoleCursorInfo(out, &cursorInfo);
}

void ListenInput(Player* P, Map* CurrMap) {
	// check for all 4 directions in this game tic
	for (direction_t dir : valid_dir) {
		if (check_dir(dir) && IsValidAction(P, CurrMap, dir))
			TakeAction(P, CurrMap, dir);
	}
}

void GameTics(int &frame) {
	g_delay(GAMETIC_CONST);
	frame++;
}

int GetMaxLevelsAllowed(GameMode gm) {
	switch (gm) {
	case GM_NORM:
	case GM_NORMAGENT:
		return NORM_LEVEL_COUNT;
	case GM_SURVIVAL:
		return SURV_LEVEL_COUNT;
	case GM_COLLECTOR:
		return COLL_LEVEL_COUNT;
	default:
		return 1;
	}
}

const point GetLevelDim(int id, GameMode gm) {
	switch (gm) {
	case GM_NORM:
//	case GM_NORMAGENT:
		return MapList_Norm_Dim[id];
	case GM_SURVIVAL:
		return MapList_Surv_Dim[id];
	case GM_COLLECTOR:
	case GM_NORMAGENT:
		return MapList_Collector_Dim[id];
	default:
		return point(0, 0);
	}
}

bool CopyMapLayout(char** temp, point dim, int id, GameMode gm) {
	switch (gm) {
	case GM_NORM:
//	case GM_NORMAGENT:
		for (int i = 0; i < dim.y; i++)
			for (int j = 0; j < dim.x; j++)
				temp[i][j] = MapList_Norm[id][i][j];
		return true;
	case GM_SURVIVAL:
		for (int i = 0; i < dim.y; i++)
			for (int j = 0; j < dim.x; j++)
				temp[i][j] = MapList_Surv[id][i][j];
		return true;
	case GM_COLLECTOR:
	case GM_NORMAGENT:
		for (int i = 0; i < dim.y; i++)
			for (int j = 0; j < dim.x; j++)
				temp[i][j] = MapList_Collector[id][i][j];
		return true;
	default:
		return false;
	}
}

void ShowInfo(Player* P, Map* CurrMap, clock_t t) {
	gotoxy(CurrMap->get_dim().x + 8, 1);
	cout << normal << "POINTS: " << P->getpts();
	if (CurrMap->get_gamemode() == GM_NORM)
		cout << " / " << CurrMap->get_points();
	gotoxy(CurrMap->get_dim().x + 8, 2);
	cout << "LEVEL:    " << CurrMap->get_levelcount() + 1;
	gotoxy(CurrMap->get_dim().x + 8, 3);
	GameTime temp(clock() - t);
	cout << "TIME: " << temp.get_time();
}

void DisplayEndScreen(Player* P, GameState gs, GameTime gt) {
	switch (gs) {
	case GS_LOST:
		cout << normal << "You lost. Your score is: " << P->getpts() << ". You've lasted " << gt.get_time() << endl;
		break;
	case GS_WON:
		cout << normal << "Congratulations! You've won the game! Your score is: " << P->getpts() << ". You spent " << gt.get_time() << endl;
		break;
	}
}

bool DisplayWonMessage(Player* P, GameTime gt) {
	clearscreen();
	cout << normal << "Good job! You've beaten this map. Continue? Y/N ";
	return Request_Key_Press(INPUT_NO, INPUT_YES);
}

bool CheckWinCondition(Player* P, Map* CurrMap) {
	switch (CurrMap->get_gamemode()) {
	case GM_NORM:
	case GM_NORMAGENT:
		if (P->getpts() == CurrMap->get_points())
			return true;
		return false;
		break;
	}
	return false;
}

bool PromptExit() {
	cout << "Press Q to exit or C to continue with a new game: ";
	return Request_Key_Press(INPUT_QUIT, INPUT_CONTINUE);
}