using System.Collections.Generic;

namespace NC_Reactor_Planner
{
    public struct HeatSinkRules
    {
        public List<Block> Adjacent;
        public List<Block> Axial;
        public List<List<Block>> Vertex;
        public List<Dictionary<Block, int>> Exact;

        public HeatSinkRules(List<Block> adjacent, List<Block> axial, List<List<Block>> vertex, List<Dictionary<Block, int>> exact)
        {
            Adjacent = adjacent;
            Axial = axial;
            Vertex = vertex;
            Exact = exact;
        }
    }
}