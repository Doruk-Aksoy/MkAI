using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MkAI
{
    class QLearn
    {
        private static double GAMMA = 0.75;
        private static int ITERATIONS = 10;
        private static int curstate = 0;
        private static int size = 2;
        private static double good_range = 2.0;
        private static double target_pos = 0;
        private static Entity assoc = null;

        private static int[] states = { 0, 1 }; // left or right
        private static int[][] R = { new int[] { -1, 0 },
                              new int[] { 0, -1 }
                            };
        private static int[,] q = new int[size, size];

        public QLearn(Entity E)
        {
            assoc = E;
        }

        public void setTarget(double pos)
        {
            target_pos = pos;
        }

        // distance between points getting less => more reward
        private static double eval(double epos, double tpos)
        {
            return tpos - epos;
        }

        public int getHighest()
        {
            int res = int.MinValue;
            for (int i = 0; i < size; ++i)
                for (int j = 0; j < size; ++j)
                    if (q[i, j] > res)
                        res = q[i, j];
            return res;

        }

        private static void initialize()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    q[i, j] = 0;
                }
            }
        }

        private static int getRandomAction(int upperBound)
        {
            int action = 0;
            bool choiceIsValid = false;

            // Randomly choose a possible action connected to the current state.
            while (choiceIsValid == false)
            {
                action = new Random().Next(0, upperBound);
                if (R[curstate][action] > -1)
                {
                    choiceIsValid = true;
                }
            }

            return action;
        }

        public static void train()
        {
            initialize();

            // Perform training, starting at all initial states.
            for (int j = 0; j < ITERATIONS; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    episode(states[i]);
                } // i
            } // j
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
        private static void episode(int initialState)
        {
            curstate = initialState;

            // Travel from state to state until goal state is reached.
            do
            {
                chooseAnAction();
            } while (eval(assoc.getPos(), target_pos) <= good_range);

            // When we meet a goal, Run through the set once more for convergence.
            for (int i = 0; i < size; i++)
            {
                chooseAnAction();
            }
            return;
        }

        private static void chooseAnAction()
        {
            int possibleAction = 0;

            // Randomly choose a possible action connected to the current state.
            possibleAction = getRandomAction(size);

            if (R[curstate][possibleAction] >= 0)
            {
                q[curstate, possibleAction] = reward(possibleAction);
                curstate = possibleAction;
            }
            return;
        }

        private static int maximum(int State, bool ReturnIndexOnly)
        {
            // If ReturnIndexOnly = True, the Q matrix index is returned.
            // If ReturnIndexOnly = False, the Q matrix value is returned.
            int winner = 0;
            bool foundNewWinner = false;
            bool done = false;

            while (!done)
            {
                foundNewWinner = false;
                for (int i = 0; i < size; i++)
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

        private static int reward(int Action)
        {
            return (int)(R[curstate][Action] + GAMMA * maximum(Action, false));
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
