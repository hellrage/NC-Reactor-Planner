using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public static class BGExportHelper
    {
        static List<int> posInt;
        static List<int> stateInt;
        static Dictionary<string, int> mapIntState;

        public static string FormBGExportString()
        {
            posInt = new List<int>();
            stateInt = new List<int>();
            mapIntState = new Dictionary<string, int>();
            List<string> preparedMapIntState = new List<string>();

            for (int y = (int)Reactor.interiorDims.Y; y > 0; y--)
                for (int z = 1; z <= Reactor.interiorDims.Z;z++)
                    for (int x = 1; x <= Reactor.interiorDims.X;x++)
                    {
                        Block block = Reactor.blocks[x, y, z];
                        string bt = block.BlockType.ToString().ToLower();
                        string ct;
                        if (bt == "cooler")
                        {
                            if (((Cooler)block).Active)
                                continue;
                            ct = ((Cooler)block).CoolerType.ToString().ToLower();
                            if (!mapIntState.ContainsKey(ct))
                                mapIntState.Add(ct, (mapIntState.Count>0)?(mapIntState.Values.Last() + 1):1);
                            stateInt.Add(mapIntState[ct]);
                            preparedMapIntState.Add("{mapSlot:" + mapIntState[ct] + "s,mapState:{Properties:{type:\"" + ct + "\"},Name:\"nuclearcraft:cooler\"}}");
                        }
                        else if (bt == "moderator")
                        {
                            ct = ((Moderator)block).ModeratorType.ToString().ToLower();
                            if (!mapIntState.ContainsKey(ct))
                                mapIntState.Add(ct, (mapIntState.Count > 0) ? (mapIntState.Values.Last() + 1) : 1);
                            stateInt.Add(mapIntState[ct]);
                            preparedMapIntState.Add("{mapSlot:"+mapIntState[ct]+"s,mapState:{Properties:{type:\""+ct.ToString().ToLower()+"\"},Name:\"nuclearcraft:ingot_block\"}}");
                        }
                        else
                        {
                            if (bt == "air")
                                continue;
                            if (!mapIntState.ContainsKey(bt))
                                mapIntState.Add(bt, (mapIntState.Count > 0) ? (mapIntState.Values.Last() + 1) : 1);
                            stateInt.Add(mapIntState[bt]);
                            preparedMapIntState.Add("{mapSlot:" + mapIntState[bt] + "s,mapState:{Name:\"nuclearcraft:cell_block\"}}");
                        }
                        int px = ((x-1) & 0xff) << 16;
                        int py = ((y-1) & 0xff) << 8;
                        int pz = (z-1) & 0xff;
                        int p = (px + py + pz);
                        posInt.Add(p);
                    }
            string startpos = "],startPos:{X:0,Y:0,Z:0},";
            string endpos = "],endPos:{" + string.Format("X:{0},Y:{1},Z:{2}", Reactor.interiorDims.X - 1, Reactor.interiorDims.Y - 1, Reactor.interiorDims.Z - 1) + "}}";
            string export = "{stateIntArray:[I;";
            export += string.Join(",", stateInt);
            export += "],dim:0,posIntArray:[I;";
            export += string.Join(",", posInt);
            export += startpos;
            export += "mapIntState:[";
            export += string.Join(",", preparedMapIntState.ToArray());
            export += endpos;
            return export;
        }
    }
}
