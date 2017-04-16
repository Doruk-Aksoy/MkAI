#ifndef AGENTMODE_IN
#define AGENTMODE_IN

#include <vcclr.h>

#include "Mobj.h"

using namespace MkAI;

class Agent {

public:

	// reference makes modifications last, important
	Agent(Player*& P, Entity^ e) : pobj_ptr(P), entity(e) { }

	Player* getPlayer() {
		return pobj_ptr;
	}

	void setLearningSystem(bool train) {
		if (train)
			entity->getEntityAI()->setLearningSystem(gcnew QLearn(entity));
		else
			entity->readLearnedData();
	}

	Entity^ getEntity() {
		return entity;
	}

	LearningSystem^ getLearningSystem() {
		return entity->getEntityAI()->getLearningSystem();
	}

private:

	// these aren't responsibility of this class -- improve later with smart ptr
	gcroot<Entity^>				entity;
	Player*&					pobj_ptr;

};

void AgentTakeInput(Agent*);
void AddStateDataFromMap(State^, Player*);
void SendRMatrix(Agent*);
int GetRewardAtPos(point&);

struct change {
	char from;
	char to;
	point p_prev;
	point p_next;
	int reward;
};

#endif