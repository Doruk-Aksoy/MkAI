using System;
using System.Collections.Generic;
using System.Linq;

namespace MkAI
{
    [Serializable]
    class StateEqualityComparer : IEqualityComparer<State>
    {
        [NonSerialized]
        public State outval;
        public bool Equals(State s1, State s2)
        {
            if (!s1.Equals(s2)) return false;
            outval = s1;
            return true;
        }

        public int GetHashCode(State s)
        {
            return s.GetHashCode();
        }
    }

    [Serializable]
    public static class HashSetExtension
    {
        public static bool TryGetValue(this HashSet<State> hs, State value, out State valout)
        {
            if (hs.Contains(value))
            {
                valout = (hs.Comparer as StateEqualityComparer).outval;
                return true;
            }
            else
            {
                valout = null;
                return false;
            }
        }
    }
}
