using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MkAI.DataTypes
{
    public class State
    {
        private string label;
        // key is used to compare uniqueness when adding to a map of unique states
        private string key;
        private int state_id;
        private int reward;
        private LearningSystem system_ref;

        public State(LearningSystem L)
        {
            label = "N/A";
            key = "Null";
            reward = 0;
            system_ref = L;
            state_id = L.getNextStateID();
            // a state was created, increment id counter
            L.incrementNextStateID();
        }

        public State(string l, string h, int r, LearningSystem L)
        {
            label = l;
            key = h;
            reward = r;
            system_ref = L;
            state_id = L.getNextStateID();
            L.incrementNextStateID();
        }

        public string getLabel()
        {
            return label;
        }

        public string getKey()
        {
            return key;
        }

        public int getReward()
        {
            return reward;
        }

        public int getID()
        {
            return state_id;
        }

        public LearningSystem getSystem()
        {
            return system_ref;
        }

        public void setReward(int r)
        {
            reward = r;
        }
    }
}
