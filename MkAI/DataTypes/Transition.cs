using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MkAI.DataTypes
{
    public class Transition
    {
        private State destination;
        private int input;
        private int reward;

        public Transition(State S, int I, int r)
        {
            destination = S;
            input = I;
            reward = r;
        }

        public Transition(Transition T)
        {
            destination = T.getDestination();
            input = T.input;
            reward = T.reward;
        }

        public State getDestination()
        {
            return destination;
        }

        public int getInput()
        {
            return input;
        }

        public bool isValidInput(int i)
        {
            return input == i;
        }

        public int getReward()
        {
            return reward;
        }

        public void setReward(int r)
        {
            reward = r;
        }
    }
}
