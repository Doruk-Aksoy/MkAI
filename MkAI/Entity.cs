using System;
using System.IO;
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
        private int id;
        private EntityAI ai;             // Contains the DeCo Network model and its data
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
    }
}
