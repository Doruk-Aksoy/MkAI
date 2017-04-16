#ifndef SYSFUNC_IN
#define SYSFUNC_IN

#include <iostream>
#include <string>
#include <sstream>
#include <Windows.h>
/*
#include <SFML/Audio.hpp>
#include <SFML/Graphics.hpp>
*/
#include "GlobalDefs.h"
#include "Mapdef.h"

using namespace std;

//								GENERIC OPERATOR OVERLOADS

ostream& operator<< (ostream&, const COLOR&);
string operator+ (string, string);

//								STANDARD DEFINITIONS

void DisplayMenu();
void DisplayDifficulty();
void DisplayAgentMode();
Map* GenerateMap(int, GameMode);
Player* GeneratePlayer(Map*);
void drawatxy(int, int, char);
GameMode AskGameMode();
Difficulty AskDifficulty();
bool AskAgentMode();

void ShowMap();
void gotoxy(short, short);
template <typename T>
string to_string(T);

template <typename T>
void g_delay(T);
bool WaitKeyPress(unsigned int);
bool Request_Key_Press(int, int);
void clearscreen();
void ShowConsoleCursor(bool);
void ListenInput(Player*, Map*);
void GameTics(int&);
int GetMaxLevelsAllowed(GameMode);
const point GetLevelDim(int, GameMode);
bool CopyMapLayout(char**, point, int, GameMode);
void ShowInfo(Player*, Map*, clock_t);
bool DisplayWonMessage(Player*, GameTime);
void DisplayEndScreen(Player*, GameState, GameTime);
bool CheckWinCondition(Player*, Map*);
bool PromptExit();

unsigned char* ShortToUChar(short);

#endif