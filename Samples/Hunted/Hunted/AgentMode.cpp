#include "AgentMode.h"
#include "GameFuncs.h"
#include "SysFunc.h"

using namespace MkAI;

using namespace System;
using namespace System::Collections::Generic;

static int state_keys = 0;

int GetRewardAtPos(point& p) {
	int result = 0;
	Item t;
	if ((t = IsItem(CurrentMap->get_tile(p.x, p.y))) != NULLITEM)
		result = 1 + static_cast<int> (t);
	else if (CurrentMap->get_tile(p.x, p.y) == EMPTY_SQUARE)
		result = 1;
	else
		result = -50;
	return result;
}

void AddStateDataFromMap(State^ S, Player* P, bool goalState) {
	// send coordinate data of player
	List<Byte>^ byte_data = gcnew List<Byte>();
	// convert short to unsigned char
	short x = P->getpos().x;
	short y = P->getpos().y;
	byte_data->Add(static_cast<unsigned char>(x & 0xff));
	byte_data->Add(static_cast<unsigned char>((x >> 8) & 0xff));
	byte_data->Add(static_cast<unsigned char>(y & 0xff));
	byte_data->Add(static_cast<unsigned char>((y >> 8) & 0xff));
	byte_data->Add(static_cast<unsigned char>(CurrentMap->)
	S->putData(byte_data);
}

void SendRMatrix(Agent* A) {
	// for every row and column, consider the new player position as a new state and send it
	Player* P = A->getPlayer();
	short width = CurrentMap->get_dim().x;
	short height = CurrentMap->get_dim().y;
	LearningSystem^ ls = A->getLearningSystem();
	for (short i = 0; i < width; ++i) {
		for (short j = 0; j < height; ++j) {
			State^ S = ls->makeState("S" + state_keys++);
			AddStateDataFromMap(S, P);
			ls->addState(S);
			// for every possible move we can do on this state, create transitions
			for (auto dir : valid_dir) {
				if (IsValidAction(P, CurrentMap, dir)) {
					short xadd, yadd;
					dir_to_int(xadd, yadd, dir); // load coords
					int x = P->getpos().x + xadd, y = P->getpos().y + yadd;
					State^ temp = ls->makeState("S" + state_keys++);
					point prev = P->getpos(), next = point(x, y);
					// simulate player position
					P->setpos(next);
					AddStateDataFromMap(temp, P);
					P->setpos(prev);
					ls->addStateTransition(S, temp, static_cast<int>(dir), GetRewardAtPos(next));
				}
			}
		}
	}
}

// This part interfaces with the C# library for decision making using the learned Q matrix
void AgentTakeInput(Agent* A) {
	Player* P = A->getPlayer();
	QLearn^ ls = dynamic_cast<QLearn^>(A->getLearningSystem());
	State^ S = ls->getCurrentState();

	// since Q matrix tells us what we have learned so far and it should give the most optimum path do:
	// pick highest reward transition for a given current state S
	// repeat this until goal state is reached
}