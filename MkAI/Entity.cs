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

    public class Entity
    {
        private string tag;
        private int id;
        private EntityAI ai;             // Contains the DeCo Network model and its data
        private static Random rnd = new Random();
        private double pos;

        protected Logger Debugger = new Logger();

        public Entity(string s)
        {
            tag = s;
            ai = new EntityAI(this);
            id = giveUniqueID();
            pos = 0;
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

        public void updatePos(double p)
        {
            pos = p;
        }

        public double getPos()
        {
            return pos;
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

        // Clean resources allocated for this Entity, not needed anymore
        public void dispose()
        {
            EntityManager.removeEntity(id);
        }

        public bool outputLearnedData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream;
            try
            {
                stream = new FileStream(tag + "_qlearn" + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (Exception E)
            {
                Debugger.Log(E.ToString() + " occured when trying to output the file.");
                return false;
            }
            formatter.Serialize(stream, ai.getLearningSystem());
            stream.Close();
            Debugger.Log("Entity data saved successfully.");
            return true;
        }

        public bool readLearnedData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream;
            try
            {
                stream = new FileStream(tag + "_qlearn" + ".bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception E)
            {
                Debugger.Log(E.ToString() + " occured when trying to read the input file.");
                return false;
            }
            ai.setLearningSystem((LearningSystem)formatter.Deserialize(stream));
            stream.Close();
            Debugger.Log("Entity data loaded successfully.");
            return true;
        }
    }
}
