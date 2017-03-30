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


        public Transition(State S, int I)
        {
            destination = S;
            input = I;
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
    }
}
