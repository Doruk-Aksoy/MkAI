using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CoLib;

namespace MkAI
{
    public static class EntityManager
    {
        public static Dictionary<int, Entity>          entity_table = new Dictionary<int, Entity>();

        public static void addEntity(Entity E)
        {
            entity_table.Add(E.getID(), E);
        }

        public static bool removeEntity(int e_id)
        {
            return entity_table.Remove(e_id);
        }

        public static Entity getEntity(int e_id)
        {
            return entity_table[e_id];
        }

        public static List<Entity> findEntitiesByTag(string tag)
        {
            List<Entity> res = new List<Entity>();

            foreach (KeyValuePair<int, Entity> e in entity_table)
                if (e.Value.getTag() == tag)
                    res.Add(e.Value);
            return res;
        }
    }
}
