#include "GameFuncs.h"
#include "Mobj.h"

void dir_to_int(short& x, short& y, direction_t dir) {
	if (dir == DIR_UP) {
		x = 0, y = -1;
	}
	else if (dir == DIR_DOWN) {
		x = 0, y = 1;
	}
	else if (dir == DIR_LEFT) {
		x = -1, y = 0;
	}
	else if (dir == DIR_RIGHT) {
		x = 1, y = 0;
	}
}

// gets keyboard input 0x8000 is for "is this key being pressed now?"
bool check_dir(direction_t dir) {
	if (dir == DIR_UP)
		return !!GetAsyncKeyState(VK_UP);
	if (dir == DIR_DOWN)
		return !!GetAsyncKeyState(VK_DOWN);
	if (dir == DIR_LEFT)
		return !!GetAsyncKeyState(VK_LEFT);
	if (dir == DIR_RIGHT)
		return !!GetAsyncKeyState(VK_RIGHT);
	return false;
}

bool IsValidAction(Player* P, Map* CurrMap, direction_t dir) {
	if (dir == DIR_NONE)
		return false;
	short xadd, yadd;
	dir_to_int(xadd, yadd, dir); // load coords
	return IsWalkable(CurrMap->get_tile(P->getpos().x + xadd, P->getpos().y + yadd));
}

int TakeAction(Player* P, Map* CurrMap, direction_t dir) {
	Item t;
	short xadd, yadd;
	dir_to_int(xadd, yadd, dir); // load coords
	short x = P->getpos().x + xadd, y = P->getpos().y + yadd;

	if ((t = IsItem(CurrMap->get_tile(x, y))) != NULLITEM) {
		CurrMap->set_tile(x, y, EMPTY_SQUARE);
		P->setxy(x, y);
		P->addpts(t);
		return 1 + static_cast<int> (t); // points is always good! (+1 so point is more rewarding than just moving, not equal)
	}
	else if (CurrMap->get_tile(x, y) == ' ') {
		P->setxy(x, y);
		return 1; // maybe good for moving towards goal?
	}
	return -5; // anything else is punishment
}

change SimulateMove(Player* P, direction_t dir) {
	change result;
	Item t;
	short xadd, yadd;
	dir_to_int(xadd, yadd, dir); // load coords
	int x = P->getpos().x + xadd, y = P->getpos().y + yadd;

	result.p_prev = point(P->getpos().x, P->getpos().y);
	result.p_next = point(x, y);
	result.from = PLAYER_FRAME;
	result.to = CurrentMap->get_tile(x, y);

	if ((t = IsItem(CurrentMap->get_tile(x, y))) != NULLITEM)
		result.reward = 1 + static_cast<int> (t);
	else if (CurrentMap->get_tile(x, y) == EMPTY_SQUARE)
		result.reward = 1;
	else
		result.reward = -50;
	return result;
}

// we're moving to point 'to', so make it the player frame, and make player's spot empty
void ApplyChange(change& C) {
	CurrentMap->set_tile(C.p_next.x, C.p_next.y, PLAYER_FRAME);
	CurrentMap->set_tile(C.p_prev.x, C.p_prev.y, EMPTY_SQUARE);
}

void RevertChange(change& C) {
	CurrentMap->set_tile(C.p_next.x, C.p_next.y, C.to);
	CurrentMap->set_tile(C.p_prev.x, C.p_prev.y, C.from);
}