using System;
using System.Collections.Generic;
using System.Linq;

// add fsm open source code, write wrapper to make it easier to use.

// this Q learning implementation uses the episodic method of learning

namespace MkAI
{
    [Serializable]
    public class QLearn : LearningSystem
    {
        private double GAMMA = 0.75;
        private int ITERATIONS = 10;
        private State prevstate = null;
        private State workstate = null;
        
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
        public Transition getCurrentHighest()
        {
            Transition res = null;
            int val = int.MinValue;
            foreach (var T in Q[curstate])
            {
                if (T.getReward() > val)
                {
                    val = T.getReward();
                    res = T;
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

        private State getRandomState()
        {
            State res = null;
            int dest = new Random().Next(0, Q[workstate].Count), pos = 0;
            foreach (State S in state_list)
            {
                // if I'm at the right one
                if (dest == pos)
                {
                    res = S;
                    break;
                }
                pos++;
            }
            return res;
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
                dest = new Random().Next(0, Q[workstate].Count);
                int pos = 0;
                foreach (Transition T in Q[workstate])
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

        override public bool train()
        {
            // Perform training, starting at all initial states.
            for (int j = 0; j < ITERATIONS; j++)
            {
                // for every iteration, attach a random goal state
                State currentgoal = getRandomState();
                addGoalState(currentgoal);
                foreach(State S in state_list)
                {
                    episode(S);
                }
                removeGoalState(currentgoal);
            }
            return true;
        }

        public bool goalReached()
        {
            // if curstate is one of the defined goal states
            foreach (State S in goal_states)
                if (S.Equals(curstate))
                    return true;
            return false;
        }

        override protected void episode(State initialState)
        {
            // if there are links
            try
            {
                if (transitions[initialState].Count > 0)
                {
                    workstate = initialState;

                    // Travel from state to state until goal state is reached.
                    do
                    {
                        chooseAnAction();
                    } while (goalReached());
                    
                    // When we meet a goal, Run through the set once more for convergence.
                    for (int i = 0; i < state_list.Count; i++)
                        chooseAnAction();
                }
            }
            catch(Exception E)
            {
                Debugger.Log("Unknown initial state exception during episode.");
            }
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

                prevstate = workstate;
                workstate = possibleAction.getDestination();
            }
        }

        // the state to process
        private int maximum(State S, bool ReturnIndexOnly)
        {
            // If ReturnIndexOnly = True, the Q matrix index is returned.
            // If ReturnIndexOnly = False, the Q matrix value is returned.
            try
            {
                Transition winner = Q[S].ElementAtOrDefault(0);
                if (winner != null)
                {
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
            }
            catch (Exception E)
            {
                Debugger.Log("Unknown state exception during maximum calculation.");
            }
            return 0;
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

        public override void setIterations(int iter)
        {
            ITERATIONS = iter;
        }

        public State getCurrentState()
        {
            return curstate;
        }

        override public bool addState(State S)
        {
            bool res = state_list.Add(S);
            if (res)
                Debugger.Log("Added State " + S.getLabel() + ".");
            else
                Debugger.Log("State " + S.getLabel() + "already exists.");
            return res;
        }

        override public bool addStateTransition(State from, State to, int input, int reward)
        {
            HashSet<Transition> temp;
            // fresh init of state -> transition table for a row
            if (!transitions.TryGetValue(from, out temp))
            {
                // we need to deepcopy the transition object because transitiond and Q hold different reward values
                Transition T = new Transition(to, input, reward);
                Transition QT = new Transition(T);
                // set reward to initially 0 -- we will learn these later
                QT.setReward(0);
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
                Transition T = new Transition(to, input, reward);
                Transition QT = new Transition(to, input, 0);
                transitions[from].Add(T);
                Q[from].Add(new Transition(QT));
                return true;
            }
        }
    }
}
