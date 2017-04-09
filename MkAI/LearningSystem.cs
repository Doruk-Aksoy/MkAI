using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MkAI
{
    using DataTypes;

    public abstract class LearningSystem
    {
        protected Entity assoc;
        protected int next_state_id = 0;
        protected State curstate = null;

        // Keep a set of goal states for checking
        protected HashSet<State> goal_states = new HashSet<State>();

        // state list contains the list of all defined states by the user
        protected HashSet<State> state_list;
        // transition list -- holds for which inputs, a transition is possible
        protected Dictionary<State, HashSet<Transition>> transitions;

        public LearningSystem(Entity e)
        {
            assoc = e;
            state_list = new HashSet<State>();
            transitions = new Dictionary<State, HashSet<Transition>>();
        }

        public void incrementNextStateID()
        {
            next_state_id++;
        }

        public int getNextStateID()
        {
            return next_state_id;
        }

        protected string getNextFreeStateLabel()
        {
            return "S" + next_state_id;
        }

        // May need to remove these two
        public State makeState(string key)
        {
            return new State(getNextFreeStateLabel(), this);
        }

        public bool addGoalState(State S)
        {
            return goal_states.Add(S);
        }

        public bool removeGoalState(State S)
        {
            return goal_states.Remove(S);
        }

        public void setCurrentState(State S)
        {
            curstate = S;
        }

        abstract public bool addState(State S);
        abstract public bool addStateTransition(State from, State to, int input, int reward);
        public abstract void initialize();
        public abstract void train();
        protected abstract void episode(State initialState);
    }
}
