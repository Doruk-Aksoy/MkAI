using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// add fsm open source code, write wrapper to make it easier to use.

// this Q learning implementation uses the episodic method of learning

namespace MkAI
{
    using MkAI.DataTypes;
    public class QLearn : LearningSystem
    {
        private double GAMMA = 0.75;
        private int ITERATIONS = 10;
        private int prevstate = 0;
        private State curstate = null;
        
        // learned values
        private Dictionary<State, HashSet<Transition>> Q;

        public QLearn(Entity E) : base(E)
        {
            Q = new Dictionary<State, HashSet<Transition>>();
            //for (int i = 0; i < size; ++i)
                //Q.Add(new List<State>());
        }

        // Returns the unique ID of the state with highest reward out of all states -- state number from 0 to N
        public int getHighest()
        {
            int res = 0;
            int val = int.MinValue;
            foreach (var S in Q)
            {
                var SVal = S.Value;
                foreach (var T in SVal)
                {
                    State temp = T.getDestination();
                    if (temp.getReward() > val)
                    {
                        val = temp.getReward();
                        res = temp.getID();
                    }
                }
            }
            return res;
        }

        // Returns the unique ID of the highest transition reward state from current state
        public int getCurrentHighest()
        {
            int res = 0;
            int val = int.MinValue;
            foreach (var T in Q[curstate])
            {
                State temp = T.getDestination();
                if (temp.getReward() > val)
                {
                    val = temp.getReward();
                    res = temp.getID();
                }
            }
            return res;
        }

        override public void initialize()
        {
            /*
             empty for now
             */
        }

        // given the upperBound for allowed action ids, pick one
        private int getRandomAction(int upperBound)
        {
            int action = 0;
            bool choiceIsValid = false;

            // Randomly choose a possible action connected to the current state.
            while (choiceIsValid == false)
            {
                action = new Random().Next(0, upperBound);
                foreach (Transition T in transitions[curstate]) // if there exists valid transition -- optimize later maybe?
                    if (T.getInput() == action)
                    {
                        choiceIsValid = true;
                        break;
                    }
            }

            return action;
        }

        override public void train()
        {
            // Perform training, starting at all initial states.
            for (int j = 0; j < ITERATIONS; j++)
            {
                foreach(State S in state_list)
                {
                    episode(S);
                } 
            }
            /*
            Console.Write("Q Matrix values:");
            for (int i = 0; i < state_size; i++)
            {
                for (int j = 0; j < state_size; j++)
                {
                    Console.Write(q[i, j] + ",\t");
                } // j
                Console.Write("\n");
            } // i
            Console.Write("\n");
            
            return;*/
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
            // if curstate is one of the defined goal states
            foreach (State S in goal_states)
                if (S.getID() == curstate.getID())
                    return true;
            return false;
        }

        override protected void episode(State initialState)
        {
            // if there are links
            if(transitions[initialState].Count > 0)
            {
                curstate = initialState;

                // Travel from state to state until goal state is reached.
                do
                {
                    chooseAnAction();
                } while (goalReached());

                // When we meet a goal, Run through the set once more for convergence.
                for (int i = 0; i < state_list.Count; i++)
                {
                    chooseAnAction();
                }
            }
            return;
        }

        private void chooseAnAction()
        {
            int possibleAction = 0;

            // Randomly choose a possible action connected to the current state.
            possibleAction = getRandomAction(state_list.Count);

            q[curstate, possibleAction] = reward(possibleAction);
            prevstate = curstate;
            curstate = possibleAction;

            // 1 = up, 2 = down, 3 = left, 4 = right -- specific for our demo
            if (curstate == 0)
                cur_y -= 1;
            else if (curstate == 1)
                cur_y += 1;
            else if (curstate == 2)
                cur_x -= 1;
            else if (curstate == 3)
                cur_x += 1;
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

        public void getReward(int val, int action)
        {
            q[prevstate, action] += val;
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
