using System;
using System.Collections.Generic;

namespace MkAI
{
    [Serializable]
    public abstract class LearningSystem
    {
        protected Entity assoc;
        [NonSerialized]
        protected int next_state_id =0;
        [NonSerialized]
        protected State curstate = null;
        [NonSerialized]
        protected Logger Debugger = null;

        // Keep a set of goal states for checking
        [NonSerialized]
        protected HashSet<State> goal_states;

        // state list contains the list of all defined states by the user
        protected HashSet<State> state_list;
        // transition list -- holds for which inputs, a transition is possible
        [NonSerialized]
        protected Dictionary<State, List<Transition>> transitions;

        public LearningSystem(Entity e)
        {
            assoc = e;
            goal_states = new HashSet<State>((new StateEqualityComparer()));
            state_list = new HashSet<State>((new StateEqualityComparer()));
            transitions = new Dictionary<State, List<Transition>>();
            Debugger = new MkAI.Logger();
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
            return new State(key, this);
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

        public State findState(List<Byte> data)
        {
            foreach (State S in state_list) {
                bool matched = true;
                for (int i = 0; matched && i < data.Count; ++i) {
                    if (data[i] != S.getData()[i])
                        matched = false;
                }
                if (matched)
                    return S;
            }
            return null;
        }

        public void setLogging(bool b)
        {
            Debugger.setLogging(b);
        }

        // Overrides for other Classes

        abstract public State addState(State S);
        abstract public bool addStateTransition(State from, State to, int input, int reward);
        public abstract void initialize();
        public abstract bool train_randomgoals();
        public abstract bool train_allstatesgoals();
        public abstract bool train_knowngoals();
        protected abstract void episode(State initialState);

        abstract public void setGamma(double g);
        abstract public void setIterations(int iterations);
    }
}
