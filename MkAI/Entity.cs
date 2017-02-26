using System;
using CoLib.DeCo;

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
    }

    public class EntityAI
    {
        private DeCoNet network;
        private DeCoInterpreter interpreter;
        private Entity e_assoc;

        public EntityAI(Entity assoc)
        {
            e_assoc = assoc;
            network = new DeCoNet();
            network.Name = e_assoc.getTag() + " (" + e_assoc.getID().ToString() + ") Network";
        }

        public DeCoUnit addUnit(DeCoUnitType type, string label, double priority)
        {
            DeCoUnit u = new DeCoUnit(type, label, priority);
            network.AddUnit(u);
            return u;
        }

        public void removeUnit(string label)
        {
            CoLib.Unit u = findByName(label);
            if (u != null)
                network.RemoveUnit(u);
        }

        public void removeUnit(CoLib.Unit u)
        {
            network.RemoveUnit(u);
        }

        public void setUnitLink(DeCoUnitLinkType e, DeCoUnit u1, DeCoUnit u2, double degree)
        {
            if (e == DeCoUnitLinkType.DeCoLink_FACILITATE)
                network.facilitate(u1, u2, degree);
            else
                network.incompatible(u1, u2, degree);
        }

        // Use after network is established
        public void setup()
        {
            interpreter = new DeCoInterpreter(network);
        }

        public void run()
        {
            interpreter.Initialize();
            interpreter.Run();
        }

        public CoLib.Unit findByName(string s)
        {
            return interpreter.Model.FindUnitByName(s);
        }

        // For the following methods, these aren't necessary if the user has access to the decounits already
        public void setPriority(string s, double p)
        {
            CoLib.Unit u = interpreter.Model.FindUnitByName(s);
            if (u != null)
                u.Priority = p;
        }

        public double getActivation(string s)
        {
            CoLib.Unit u = interpreter.Model.FindUnitByName(s);
            if (u != null)
                return u.Activation;
            return -1;
        }
    }
}
