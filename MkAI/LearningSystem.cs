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
        protected int target_state = 0;
        protected int state_size = 2;
        protected int next_state_id = 0;

        // Keep a list of goal states for checking
        protected List<State> goal_states = new List<State>();

        // state list -- uses adj list to represent state transitions
        // also holds initial values -- treat as R matrix
        protected Dictionary<State, List<State>> state_list;

        public LearningSystem(Entity e, int size)
        {
            assoc = e;
            state_size = size;
            state_list = new Dictionary<State, List<State>>();
        }

        public void SetStateSize(int s)
        {
            state_size = s;
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

        public State makeState(string hash, int reward)
        {
            return new State(getNextFreeStateLabel(), hash, reward, this);
        }

        public State makeState(string label, string hash, int reward)
        {
            return new State(label, hash, reward, this);
        }

        public void setGoal(int state_id)
        {
            if (state_id < state_size)
                target_state = state_id;
        }

        public void addGoalState(State S)
        {
            goal_states.Add(S);
        }

        public void addGoalStateCheck(State S)
        {
            if (!goal_states.Exists(x => x.getID() == S.getID()))
                goal_states.Add(S);
        }

        public void addStateTransition(State from, State to)
        {
            if(state_list[from.getID()].Exists())
        }

        public abstract void initialize();
        public abstract void train();
        protected abstract void episode(int initialState);
    }
}
