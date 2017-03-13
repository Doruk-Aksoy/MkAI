using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MkAI
{
    public class QLearn : LearningSystem
    {
        private double GAMMA = 0.75;
        private int ITERATIONS = 10;
        private int curstate = 0;

        private int dest_x, dest_y;
        private int cur_x, cur_y;

        // up down left right -> 1 2 3 4

        // actions we can take
        private int[] states;

        // initial values
        private int[,] R;
        // learned values
        private int[,] q;

        public QLearn(Entity E, int size) : base(E, size)
        {
            states = new int[size];
            R = new int[size, size];
            q = new int[size, size];
        }

        // set the target which should be our destination
        public void setGoal(int x, int y)
        {
            dest_x = x;
            dest_y = y;
        }

        public void setCurPos(int x, int y)
        {
            cur_x = x;
            cur_y = y;
        }

        // distance between points getting less => more reward
        private static double eval(double epos, double tpos)
        {
            return tpos - epos;
        }

        public int getHighest()
        {
            int res = int.MinValue;
            for (int i = 0; i < state_size; ++i)
                for (int j = 0; j < state_size; ++j)
                    if (q[i, j] > res)
                        res = q[i, j];
            return res;
        }

        override public void initialize()
        {
            for (int i = 0; i < state_size; i++)
            {
                for (int j = 0; j < state_size; j++)
                {
                    q[i, j] = 0;
                }
            }
        }

        private int getRandomAction(int upperBound)
        {
            int action = 0;
            bool choiceIsValid = false;

            // Randomly choose a possible action connected to the current state.
            while (choiceIsValid == false)
            {
                action = new Random().Next(0, upperBound);
                if (R[curstate, action] > -1)
                {
                    choiceIsValid = true;
                }
            }

            return action;
        }

        override public void train()
        {
            initialize();

            // Perform training, starting at all initial states.
            for (int j = 0; j < ITERATIONS; j++)
            {
                for (int i = 0; i < state_size; i++)
                {
                    episode(states[i]);
                } 
            }
            /*
            System.out.println("Q Matrix values:");
            for (int i = 0; i < Q_SIZE; i++)
            {
                for (int j = 0; j < Q_SIZE; j++)
                {
                    System.out.print(q[i][j] + ",\t");
                } // j
                System.out.print("\n");
            } // i
            System.out.print("\n");
            */
            return;
        }
        /*
        private static void test()
        {
            // Perform tests, starting at all initial states.
            System.out.println("Shortest routes from initial states:");
            for (int i = 0; i < Q_SIZE; i++)
            {
                currentState = INITIAL_STATES[i];
                int newState = 0;
                do
                {
                    newState = maximum(currentState, true);
                    System.out.print(currentState + ", ");
                    currentState = newState;
                } while (currentState < 5);
                System.out.print("5\n");
            }

            return;
        }
        */

        public bool goalReached()
        {
            return cur_x == dest_x && cur_y == dest_y;
        }

        override protected void episode(int initialState)
        {
            curstate = initialState;

            // Travel from state to state until goal state is reached.
            do
            {
                chooseAnAction();
            } while (!goalReached());

            // When we meet a goal, Run through the set once more for convergence.
            for (int i = 0; i < state_size; i++)
            {
                chooseAnAction();
            }
            return;
        }

        private void chooseAnAction()
        {
            int possibleAction = 0;

            // Randomly choose a possible action connected to the current state.
            possibleAction = getRandomAction(state_size);

            if (R[curstate, possibleAction] >= 0)
            {
                q[curstate, possibleAction] = reward(possibleAction);
                curstate = possibleAction;

                // 1 = up, 2 = down, 3 = left, 4 = right -- specific for our demo
                if (curstate == 1)
                    cur_y -= 1;
                else if (curstate == 2)
                    cur_y += 1;
                else if (curstate == 3)
                    cur_x -= 1;
                else if (curstate == 4)
                    cur_x += 1;
            }
            return;
        }

        private int maximum(int State, bool ReturnIndexOnly)
        {
            // If ReturnIndexOnly = True, the Q matrix index is returned.
            // If ReturnIndexOnly = False, the Q matrix value is returned.
            int winner = 0;
            bool foundNewWinner = false;
            bool done = false;

            while (!done)
            {
                foundNewWinner = false;
                for (int i = 0; i < state_size; i++)
                {
                    if (i != winner)
                    {             // Avoid self-comparison.
                        if (q[State, i] > q[State, winner])
                        {
                            winner = i;
                            foundNewWinner = true;
                        }
                    }
                }

                if (foundNewWinner == false)
                {
                    done = true;
                }
            }

            if (ReturnIndexOnly == true)
                return winner;
            return q[State, winner];
        }

        private int reward(int Action)
        {
            return (int)(R[curstate, Action] + GAMMA * maximum(Action, false));
        }

        public double getGamma()
        {
            return GAMMA;
        }

        public int getIterations()
        {
            return ITERATIONS;
        }
    }
}
