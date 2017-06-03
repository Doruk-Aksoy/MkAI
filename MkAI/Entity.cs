using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MkAI
{
    public enum DeCoUnitLinkType
    {
        DeCoLink_FACILITATE,
        DeCoLink_INCOMPATIBLE
    }

    public enum LearningSystemType
    {
        LS_QLEARNING
    }

    [Serializable]
    public class Entity
    {
        private string tag;
        private Int32 id;
        private EntityAI ai;             // Contains the DeCo Network model and its data
        [NonSerialized]
        private static Random rnd = new Random();
        private LearningSystemType lstype;

        protected Logger Debugger = new Logger();

        public Entity(string s)
        {
            tag = s;
            ai = new EntityAI(this);
            id = giveUniqueID();
            EntityManager.entity_table.Add(id, this);
        }

        public Int32 getID()
        {
            return id;
        }

        public string getTag()
        {
            return tag;
        }

        public EntityAI getEntityAI()
        {
            return ai;
        }

        public void setTag(string s)
        {
            tag = s;
        }

        private int giveUniqueID()
        {
            int e_id = 0;
            do
            {
                e_id = rnd.Next(0, int.MaxValue);
            } while (EntityManager.entity_table.ContainsKey(e_id));
            return e_id;
        }

        public void setLearningSystemType(LearningSystemType type)
        {
            lstype = type;
        }

        // Clean resources allocated for this Entity, not needed anymore
        public void dispose()
        {
            EntityManager.removeEntity(id);
        }

        public bool exportLearnedData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream;
            try
            {
                stream = new FileStream(tag + "_data" + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (Exception E)
            {
                Debugger.Log(E.ToString() + " occured when trying to output the file.");
                return false;
            }
            formatter.Serialize(stream, ai.getLearningSystem());
            stream.Close();
            Debugger.Log("Learning System data saved successfully.");
            return true;
        }

        public bool exportLearnedData_TXT()
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(tag + "_data" + ".txt", true))
            {
                file.WriteLine(lstype.ToString());
                LearningSystem ls = ai.getLearningSystem();
                // State list
                foreach(var S in ls.getStateList())
                {
                    file.Write(S.getID() + " " + S.getLabel() + " ");
                    Byte[] bytes = S.getData();
                    for (int i = 0; i < bytes.Length; ++i)
                        file.Write(bytes[i].ToString() + " ");
                    file.Write(Environment.NewLine);
                }
                file.Write(Environment.NewLine);
                // check types
                // if Q learn, put Q matrix
                if (lstype == LearningSystemType.LS_QLEARNING)
                {
                    QLearn lsq = (QLearn) ls;
                    Dictionary<State, List<Transition>> QMatrix = lsq.getQMatrix();
                    foreach(State S in lsq.getStateList())
                    {
                        file.Write(S.getID() + " [ ");
                        int cur = 0;
                        foreach (Transition T in QMatrix[S])
                        {
                            file.Write("( " + T.getDestination().getID() + " " + T.getInput() + " " + T.getReward());
                            if (cur == QMatrix[S].Count - 1)
                                file.Write(" ) ]");
                            else
                                file.Write(" ), ");
                            ++cur;
                        }
                        file.Write(Environment.NewLine);
                    }
                }
            }
            return true;
        }

        public LearningSystem readLearnedData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream;
            try
            {
                stream = new FileStream(tag + "_data" + ".bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception E)
            {
                Debugger.Log(E.ToString() + " occured when trying to read the input file.");
                return null;
            }
            LearningSystem res = null;
            // try some factory pattern here later
            if(lstype == LearningSystemType.LS_QLEARNING)
                res = (QLearn)formatter.Deserialize(stream);
            stream.Close();
            Debugger.Log("Entity learning data loaded successfully.");
            return res;
        }

        public LearningSystem readLearnedData_TXT()
        {
            LearningSystem ls = null;
            using (System.IO.StreamReader file =
            new System.IO.StreamReader(tag + "_data" + ".txt", true))
            {
                string buf = file.ReadLine();
                if (buf.Equals(LearningSystemType.LS_QLEARNING.ToString()))
                {
                    ls = new QLearn(this);
                    // read till newline => Q matrix
                    while ((buf = file.ReadLine()).Length > 1)
                    {
                        int counter = 0;
                        State toAdd = new State();
                        List<Byte> bytes = new List<Byte>();
                        foreach (var S in buf.Split(' '))
                        {
                            if (counter == 0)
                            {
                                int tmp = 0;
                                int.TryParse(S, out tmp);
                                toAdd.setID(tmp);
                                Debugger.Log("ID Read: " + tmp);
                            }
                            else if (counter == 1)
                            {
                                toAdd.setLabel(S);
                                Debugger.Log("Label Read: " + S);
                            }
                            else
                            {
                                int btemp;
                                int.TryParse(S, out btemp);
                                byte b = (byte) (btemp & 0xff);
                                bytes.Add(b);
                                Debugger.Log("Adding byte: " + b);
                            }
                            ++counter;
                        }
                        toAdd.putData(bytes);
                        toAdd.setRef(ls);
                        ls.addState(toAdd);
                    }
                    while((buf = file.ReadLine()) != null)
                    {
                        // get index of state
                        int index = 0;
                        string tmp_index = buf.Split(' ')[0];
                        int.TryParse(tmp_index, out index);
                        State toAdd = ls.findState(index);
                        Debugger.Log("Read State index: " + index);
                        // extract transition objects
                        foreach(var S in buf.Split('(', ')').Where((item, t) => item.Length >= 5 && item[item.Length - 2] != ']' && item[item.Length - 2] != '[').ToList())
                        {
                            int dest = 0, input = 0, reward = 0;
                            // gather all values with numbers
                            string[] content = Array.ConvertAll(S.Split(' '), p => p.Trim()).Where((item, t) => item.Any(char.IsDigit)).ToArray();
                            int.TryParse(content[0], out dest);
                            int.TryParse(content[1], out input);
                            int.TryParse(content[2], out reward);
                            Debugger.Log("Transition Info -> Destination: " + dest + " Input: " + input + " Reward: " + reward);
                            State destination = ls.findState(dest);
                            if(destination != null)
                                ((QLearn)ls).addStateTransition_Load(toAdd, destination, input, reward);
                        }
                    }
                }
            }
            return ls;
        }
    }
}
