#ifndef GAMEFUNC_IN
#define GAMEFUNC_IN

#include "GlobalDefs.h"
#include "AgentMode.h"

bool check_dir(direction_t);
void dir_to_int(short&, short&, direction_t);
bool IsValidAction(Player*, Map*, direction_t);
int TakeAction(Player*, Map*, direction_t);
void AgentTakeInput(Agent*, Map*);


change SimulateMove(Player*, direction_t);
void ApplyChange(change&);
void RevertChange(change&);

#endif
