﻿using System;
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
        private State prevstate = null;
        private State curstate = null;
        
        // learned values -- make this into list of integer stuff with copying!
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
                    if (T.getReward() > val)
                    {
                        val = T.getReward();
                        res = T.getDestination().getID();
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
                if (T.getReward() > val)
                {
                    val = T.getReward();
                    res = T.getDestination().getID();
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
        private Transition getRandomAction()
        {
            int dest = 0;
            bool choiceIsValid = false;
            Transition res = null;

            // Randomly choose a possible action connected to the current state.
            while (choiceIsValid == false)
            {
                // upperbound is the upperbound for an input for the transition
                dest = new Random().Next(0, Q[curstate].Count);
                int pos = 0;
                foreach (Transition T in Q[curstate])
                {
                    // if I'm at the right one
                    if (dest == pos)
                    {
                        choiceIsValid = true;
                        res = T;
                        break;
                    }
                    pos++;
                }
            }
            return res;
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
            Transition possibleAction = null;

            // Randomly choose a possible action connected to the current state.
            possibleAction = getRandomAction();
            if (possibleAction != null)
            {
                // update the reward
                possibleAction.setReward(reward(possibleAction));

                prevstate = curstate;
                curstate = possibleAction.getDestination();
            }
        }

        // the state to process
        private int maximum(State S, bool ReturnIndexOnly)
        {
            // If ReturnIndexOnly = True, the Q matrix index is returned.
            // If ReturnIndexOnly = False, the Q matrix value is returned.
            Transition winner = Q[S].ElementAtOrDefault(0);
            bool foundNewWinner = false;
            bool done = false;

            while (!done)
            {
                foundNewWinner = false;
                foreach (Transition T in Q[S])
                {
                    if (T != winner) // Avoid self-comparison.
                    {
                        if (T.getReward() > winner.getReward())
                        {
                            winner = T;
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
                return winner.getDestination().getID();
            return winner.getReward();
        }

        private int reward(Transition T)
        {
            return (int)(T.getReward() + GAMMA * maximum(T.getDestination(), false));
        }

        public double getGamma()
        {
            return GAMMA;
        }

        public int getIterations()
        {
            return ITERATIONS;
        }

        override public bool addState(State S)
        {
            return state_list.Add(S);
        }

        override public bool addStateTransition(State from, State to, int input, int reward)
        {
            HashSet<Transition> temp;
            // fresh init of state -> transition table for a row
            if(!transitions.TryGetValue(from, out temp))
            {
                // we need to deepcopy the transition object because transitiond and Q hold different reward values
                Transition T = new Transition(to, input, reward);
                Transition QT = new DataTypes.Transition(T);
                temp = new HashSet<Transition>();
                HashSet<Transition> qtemp = new HashSet<Transition>();
                temp.Add(T);
                qtemp.Add(QT);
                transitions.Add(from, temp);
                Q.Add(from, qtemp);
                return false;
            }
            else
            {
                Transition T = new DataTypes.Transition(to, input, reward);
                transitions[from].Add(T);
                Q[from].Add(new Transition(T));
                return true;
            }
        }
    }
}
