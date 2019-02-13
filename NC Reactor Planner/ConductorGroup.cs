using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public class ConductorGroup
    {
        public int GroupID;
        public List<Conductor> conductors;
        public bool HasPathToCasing;

        public ConductorGroup(int id)
        {
            GroupID = id;
            HasPathToCasing = false;
            conductors = new List<Conductor>();
        }
    }
}
