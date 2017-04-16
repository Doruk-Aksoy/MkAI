using CoLib.DeCo;

namespace MkAI
{
    public class EntityAI
    {
        private DeCoNet network;
        private DeCoInterpreter interpreter;
        private LearningSystem ls = null;
        private Entity e_assoc;

        public EntityAI(Entity assoc)
        {
            e_assoc = assoc;
            network = new DeCoNet(); // change later to allow options to just build this
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

        public LearningSystem getLearningSystem()
        {
            return ls;
        }

        public void setLearningSystem(LearningSystem nls)
        {
            ls = nls;
        }
    }
}
