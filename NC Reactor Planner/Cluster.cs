using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public class Cluster
    {
        public bool HasPathToCasing { get; set; }
        public double TotalHeat { get; private set; }
        public double TotalCooling { get; private set; }
        public int ID { get; private set; }

        public List<Block> blocks;

        public Cluster(int id)
        {
            blocks = new List<Block>();
            TotalCooling = 0;
            TotalHeat = 0;
            ID = id;
        }

        public void AddBlock(Block block)
        {
            blocks.Add(block);
        }
    }
}
