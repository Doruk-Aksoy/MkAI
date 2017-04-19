using System;
using System.Collections.Generic;

namespace MkAI
{
    [Serializable]
    public class State
    {
        private string label;
        // key is used to compare uniqueness when adding to a map of unique states
        private Byte[] data = null;
        private int state_id;
        private LearningSystem system_ref;

        // copy byte data to this later with another method
        public State(string l, LearningSystem L)
        {
            label = l;
            data = null;
            system_ref = L;
            state_id = L.getNextStateID();
            L.incrementNextStateID();
        }

        public State(State S)
        {
            label = S.label;
            data = new Byte[S.data.Length];
            S.data.CopyTo(data, 0);
            state_id = S.getID();
            system_ref = S.getSystem();
        }

        public void putData(List<Byte> b)
        {
            data = new Byte[b.Count];
            for (int i = 0; i < b.Count; ++i)
                data[i] = b[i];
        }

        public string getLabel()
        {
            return label;
        }

        public byte[] getData()
        {
            return data;
        }

        public int getID()
        {
            return state_id;
        }

        public LearningSystem getSystem()
        {
            return system_ref;
        }

        // compare data contents (optimize with specialized State equality comparison)
        override public bool Equals(Object S)
        {
            if (S == null || data == null || ((State)S).data == null)
                return false;
            if (data.Length != ((State)S).data.Length)
                return false;
            for (int i = 0; i < data.Length; ++i)
                if (data[i] != ((State)S).data[i])
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            return data.Length;
        }
    }
}
