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
        private LearningSystem system_ref;

        public State(LearningSystem L)
        {
            label = "N/A";
            key = "Null";
            system_ref = L;
            state_id = L.getNextStateID();
            // a state was created, increment id counter
            L.incrementNextStateID();
        }

        public State(string l, string h, LearningSystem L)
        {
            label = l;
            key = h;
            system_ref = L;
            state_id = L.getNextStateID();
            L.incrementNextStateID();
        }

        public State(State S)
        {
            label = S.label;
            key = S.getKey();
            state_id = S.getID();
            system_ref = S.getSystem();
        }

        public string getLabel()
        {
            return label;
        }

        public string getKey()
        {
            return key;
        }

        public int getID()
        {
            return state_id;
        }

        public LearningSystem getSystem()
        {
            return system_ref;
        }
    }
}
