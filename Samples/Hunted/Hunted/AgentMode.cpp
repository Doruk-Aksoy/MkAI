#include "AgentMode.h"
#include "GameFuncs.h"
#include "SysFunc.h"

using namespace MkAI;

static int state_keys = 0;

int GetRewardAtPos(point& p) {
	int result = 0;
	Item t;
	if ((t = IsItem(CurrentMap->get_tile(p.x, p.y))) != NULLITEM)
		result = 1 + static_cast<int> (t);
	else if (CurrentMap->get_tile(p.x, p.y) == EMPTY_SQUARE || CurrentMap->get_tile(p.x, p.y) == PLAYER_FRAME)
		result = 0;
	else
		result = -50;
	return result;
}

List<Byte>^ PlayerCoordToByteList(Player* P) {
	List<Byte>^ byte_data = gcnew List<Byte>();
	short x = P->getpos().x;
	short y = P->getpos().y;
	byte_data->Add(static_cast<unsigned char>(x & 0xff));
	byte_data->Add(static_cast<unsigned char>((x >> 8) & 0xff));
	byte_data->Add(static_cast<unsigned char>(y & 0xff));
	byte_data->Add(static_cast<unsigned char>((y >> 8) & 0xff));
	return byte_data;
}

void AddStateDataFromMap(State^ S, Player* P, bool goalState) {
	// send coordinate data of player
	List<Byte>^ byte_data = PlayerCoordToByteList(P);
	S->putData(byte_data);
}

void SendRMatrix(Agent* A) {
	// for every row and column, consider the new player position as a new state and send it
	Player* P = A->getPlayer();
	short width = CurrentMap->get_dim().x;
	short height = CurrentMap->get_dim().y;
	LearningSystem^ ls = A->getLearningSystem();
	for (short i = 0; i < height; ++i) {
		for (short j = 0; j < width; ++j) {
			// try to add this state if and only if it's actually possible to get there!
			point base(j, i);
			if (IsWalkable(CurrentMap->get_tile(base.x, base.y))) {
				State^ S = ls->makeState("S" + state_keys++);
				P->setpos(base);
				AddStateDataFromMap(S, P, false);
				ls->addState(S);
				// for every possible move we can do on this state, create transitions
				for (auto dir : valid_dir) {
					if (IsValidAction(P, CurrentMap, dir)) {
						short xadd, yadd;
						dir_to_int(xadd, yadd, dir); // load coords
						short x = P->getpos().x + xadd, y = P->getpos().y + yadd;
						State^ temp = ls->makeState("S" + state_keys++);
						point prev = P->getpos(), next = point(x, y);
						// simulate player position
						P->setpos(next);
						AddStateDataFromMap(temp, P, false);
						P->setpos(prev);
						ls->addState(temp);
						ls->addStateTransition(S, temp, static_cast<int>(dir), GetRewardAtPos(next));
					}
				}
			}
		}
	}
}

// This part interfaces with the C# library for decision making using the learned Q matrix
void AgentTakeInput(Agent* A) {
	Player* P = A->getPlayer();
	QLearn^ ls = (QLearn^)(A->getLearningSystem());
	State^ S = ls->getCurrentState();
	if (S == nullptr) {
		S = ls->findState(PlayerCoordToByteList(P));
		ls->setCurrentState(S);
	}
	// since Q matrix tells us what we have learned so far and it should give the most optimum path do:
	// pick highest reward transition for a given current state S
	// repeat this until goal state is reached
	Transition^ toTake = ls->getCurrentHighest();
	if (toTake != nullptr) {
		TakeAction(P, CurrentMap, static_cast<direction_t>(toTake->getInput()));
		ls->setCurrentState(toTake->getDestination());
	}
}