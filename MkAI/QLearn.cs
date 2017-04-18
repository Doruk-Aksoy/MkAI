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
        [NonSerialized]
        private double GAMMA = 0.75;
        [NonSerialized]
        private int ITERATIONS = 10;
        [NonSerialized]
        private State workstate = null;
        
        // learned values -- make this into list of integer stuff with copying!
        private Dictionary<State, HashSet<Transition>> Q;

        public QLearn(Entity E) : base(E)
        {
            Q = new Dictionary<State, HashSet<Transition>>();
            E.setLearningSystemType(LearningSystemType.LS_QLEARNING);
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
            int dest = new Random().Next(0, state_list.Count), pos = 0;
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
        // return the corresponding transition pair from R and Q matrices
        private Transition getRandomAction(out Transition Qres)
        {
            int dest = 0;
            bool choiceIsValid = false;
            Transition Rres = null;

            // Randomly choose a possible action connected to the current state.
            Qres = null;
            while (choiceIsValid == false)
            {
                int pos = 0;
                // upperbound is the upperbound for an input for the transition
                dest = new Random().Next(0, transitions[workstate].Count);
                foreach (Transition T in transitions[workstate])
                {
                    // if I'm at the right one
                    if (dest == pos)
                    {
                        choiceIsValid = true;
                        Rres = T;
                        break;
                    }
                    pos++;
                }
                // return the corresponding Q one as well (in the future, improve this part as it can cause performance issues)
                foreach (Transition T in Q[workstate])
                {
                    // if I'm at the right one
                    if (T.getDestination().Equals(Rres.getDestination()))
                    {
                        Qres = T;
                        break;
                    }
                }
            }
            return Rres;
        }

        override public bool train_allstatesgoals()
        {
            // Perform training, starting at all initial states.
            for (int j = 0; j < ITERATIONS; j++)
            {
                // for every iteration, check every state there is
                foreach (State G in state_list)
                {
                    addGoalState(G);
                    foreach (State S in state_list)
                        episode(S);
                    removeGoalState(G);
                }
            }
            return true;
        }

        override public bool train_randomgoals()
        {
            // Perform training, starting at all initial states.
            for (int j = 0; j < ITERATIONS; j++)
            {
                State G = getRandomState();
                addGoalState(G);
                foreach (State S in state_list)
                    episode(S);
                removeGoalState(G);
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
                    
                    // When we meet a goal, run through the set once more for convergence.
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
            Transition toUpdate = null;
            // Randomly choose a possible action connected to the current state.
            possibleAction = getRandomAction(out toUpdate);
            if (possibleAction != null)
            {
                // update the reward
                toUpdate.setReward(reward(possibleAction));
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
                            done = true;
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

        public override void setGamma(double g)
        {
            if (g >= 0.0f && g <= 1.0f)
                GAMMA = g;
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
                Debugger.Log("State " + S.getLabel() + " already exists.");
            return res;
        }

        override public bool addStateTransition(State from, State to, int input, int reward)
        {
            HashSet<Transition> temp;
            // fresh init of state -> transition table for a row
            if (!transitions.TryGetValue(from, out temp))
            {
                // we need to deepcopy the transition object because transitions and Q hold different reward values
                Transition T = new Transition(to, input, reward);
                Transition QT = new Transition(to, input, reward);
                // set reward to initially 0 -- we will learn these later
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
                Q[from].Add(QT);
                return true;
            }
        }
    }
}
