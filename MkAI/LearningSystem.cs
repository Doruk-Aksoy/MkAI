using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MkAI
{
    public abstract class LearningSystem
    {
        protected Entity assoc;
        protected int state_size = 2;

        public LearningSystem()
        {
            assoc = null;
        }

        public LearningSystem(Entity e, int size)
        {
            assoc = e;
            state_size = size;
        }

        public void SetStateSize(int s)
        {
            state_size = s;
        }

        public abstract void initialize();
        public abstract void train();
        protected abstract void episode(int initialState);
    }
}
