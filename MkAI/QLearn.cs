using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

// add fsm open source code, write wrapper to make it easier to use.

// this Q learning implementation uses the episodic method of learning

namespace MkAI
{
    [Serializable]
    public class QLearn : LearningSystem
    {
        // this is for maximum random rolls to prevent getting systems stuck -- change in future to make sure less than max moves in a state
        [NonSerialized]
        const int MAX_RANDOM_ITERATIONS = 256;
        [NonSerialized]
        private double GAMMA = 0.75;
        [NonSerialized]
        private int iterations = 10;
        [NonSerialized]
        private State workstate = null;
        
        // learned values -- make this into list of integer stuff with copying!
        private Dictionary<State, List<Transition>> Q;
        // consider a list of prohibited states for an episode, to mark them as already visited and not bother with them
        [NonSerialized]
        private bool endEpisode = false;
        [NonSerialized]
        private List<State> prohibited; // write extension later to prevent deep copy of states on add operation

        public QLearn(Entity E) : base(E)
        {
            Q = new Dictionary<State, List<Transition>>();
            prohibited = new List<State>();
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

        public List<Transition> getCurrentTransitions()
        {
            return Q[curstate];
        }

        public Dictionary<State, List<Transition>> getQMatrix()
        {
            return Q;
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
            int dest = 0, max_iter = 0;
            Transition Rres = null;
            // Randomly choose a possible action connected to the current state.
            Qres = null;
            // upperbound is for an input for the transition
            do
            {
                dest = new Random().Next(0, transitions[workstate].Count);
                max_iter++;
            } while (max_iter < MAX_RANDOM_ITERATIONS && prohibited.Contains(transitions[workstate][dest].getDestination()));
            // check if we must end the episode due to getting stuck
            if (max_iter == MAX_RANDOM_ITERATIONS)
            {
                //Debugger.Log("Episode halted due to maximum iteration being reached.");
                endEpisode = true;
            }
            else
            {
                Rres = transitions[workstate][dest];
                Qres = Q[workstate][dest];
            }
            return Rres;
        }

        override public bool train_allstatesgoals()
        {
            LogSystemConfig();
            OutputR();

            // Perform training, starting at all initial states.
            for (int j = 0; j < iterations; j++)
            {
                // for every iteration, check every state there is
                foreach (State G in state_list)
                {
                    addGoalState(G);
                    Debugger.Log("Iteration #" + (j + 1) + " for state " + G.getLabel() + " started.");
                    foreach (State S in state_list)
                    {
                        reset_prohibitions();
                        episode(S);
                    }
                    Debugger.Log("Iteration #" + (j + 1) + " finished.");
                    removeGoalState(G);
                }
            }

            OutputQ();
            return true;
        }

        override public bool train_randomgoals()
        {
            LogSystemConfig();
            OutputR();

            // Perform training, starting at all initial states.
            for (int j = 0; j < iterations; j++)
            {
                State G = getRandomState();
                addGoalState(G);
                Debugger.Log("Iteration #" + (j + 1) + " started.");
                foreach (State S in state_list)
                {
                    reset_prohibitions();
                    episode(S);
                }
                Debugger.Log("Iteration #" + (j + 1) + " finished.");
                removeGoalState(G);
            }

            OutputQ();
            return true;
        }

        override public bool train_knowngoals()
        {
            LogSystemConfig();
            OutputR();

            // Perform training, starting at all initial states.
            for (int j = 0; j < iterations; j++)
            {
                Debugger.Log("Iteration #" + (j + 1) + " started.");
                foreach (State S in state_list)
                {
                    reset_prohibitions();
                    episode(S);
                }
                Debugger.Log("Iteration #" + (j + 1) + " finished.");
            }

            OutputQ();
            return true;
        }

        public bool goalReached()
        {
            // if current state is one of the defined goal states
            foreach (State S in goal_states)
            {
                if (S.Equals(workstate))
                    return true;
            }
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
                    prohibited.Add(workstate);
                    // Travel from state to state until goal state is reached.
                    do
                    {
                        chooseAnAction();
                    } while (!endEpisode && !goalReached());

                    if (!endEpisode)
                    {
                        // When we meet a goal, run through the set once more for convergence.
                        for (int i = 0; i < state_list.Count; i++)
                            chooseAnAction();
                    }
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
                prohibited.Add(workstate);
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
                            if (T.getReward() > winner.getReward())
                            {
                                winner = T;
                                foundNewWinner = true;
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
            return iterations;
        }

        public override void setGamma(double g)
        {
            if (g >= 0.0f && g <= 1.0f)
                GAMMA = g;
        }

        public override void setIterations(int iter)
        {
            iterations = iter;
        }

        public State getCurrentState()
        {
            return curstate;
        }

        // null if state wasn't there before but returns the actual state if it exists
        override public State addState(State S)
        {
            State temp;
            if((temp = findState(S.getData())) == null)
            {
                state_list.Add(S);
                Debugger.Log("Added State " + S.getLabel() + ".");
                return null;
            }
            else
                Debugger.Log("State " + S.getLabel() + " already exists.");
            return temp;
        }

        override public bool addStateTransition(State from, State to, int input, int reward)
        {
            List<Transition> temp;
            // fresh init of state -> transition table for a row
            if (!transitions.TryGetValue(from, out temp))
            {
                // we need to keep seperate copies because these will differ in the reward values
                Transition T = new Transition(to, input, reward);
                Transition QT = new Transition(to, input, 0);
                temp = new List<Transition>();
                List<Transition> qtemp = new List<Transition>();
                temp.Add(T);
                qtemp.Add(QT);
                transitions.Add(from, temp);
                Q.Add(from, qtemp);
                Debugger.Log("Transition added. From " + from.getID() + " to " + to.getID() + ".");
                return false;
            }
            else
            {
                Transition T = new Transition(to, input, reward);
                Transition QT = new Transition(to, input, 0);
                transitions[from].Add(T);
                Q[from].Add(QT);
                Debugger.Log("Transition added. From " + from.getID() + " to " + to.getID() + ".");
                return true;
            }
        }

        // used for loading directly for data read_txt on Entity
        public bool addStateTransition_Load(State from, State to, int input, int reward)
        {
            List<Transition> temp;
            // fresh init of state -> transition table for a row
            if (!Q.TryGetValue(from, out temp))
            {
                Transition T = new Transition(to, input, reward);
                temp = new List<Transition>();
                temp.Add(T);
                Q.Add(from, temp);
                return false;
            }
            else
            {
                Transition T = new Transition(to, input, reward);
                Q[from].Add(T);
                return true;
            }
        }

        public void reduceRewardAllTransitionToState(State S, double factor)
        {
            foreach(State s in state_list) {
                foreach (Transition T in Q[s])
                {
                    if (T.getDestination().Equals(S))
                        T.setReward((int)(T.getReward() * factor));
                }
            }
        }

        private void reset_prohibitions()
        {
            prohibited.Clear();
            endEpisode = false;
        }

        private void LogSystemConfig()
        {
            Debugger.Log("Iterations = " + iterations + " Gamma = " + GAMMA);
        }

        private void OutputR()
        {
            Debugger.Log("R MATRIX\n------------------");
            foreach (State S in state_list)
            {
                string s = S.getLabel() + " -> ";
                try
                {
                    foreach (Transition T in transitions[S])
                    {
                        s += T.getDestination().getLabel() + "<" + T.getReward() + ">  ";
                    }
                }
                catch (Exception E)
                {
                    Debugger.Log("No transitions.");
                }
                Debugger.Log(s);
            }
        }

        private void OutputQ()
        {
            Debugger.Log("Q MATRIX\n------------------");
            foreach (State S in state_list)
            {
                string s = S.getLabel() + " -> ";
                try
                {
                    foreach (Transition T in Q[S])
                    {
                        s += T.getDestination().getLabel() + "<" + T.getReward() + ">  ";
                    }
                }
                catch (Exception E)
                {
                    s += "No transitions.";
                }
                Debugger.Log(s);
            }
        }
    }
}
