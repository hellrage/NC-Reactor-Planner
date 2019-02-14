using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    [Serializable()]
    public class HeatSink : Block
    {
        private double _cooling;
        private string _requirements;
        private bool _oldValid;
        private bool _valid;
        private bool _active;
        private List<string> placementErrors;

        private HeatSinkTypes _coolerType;
        
        public double Cooling { get => _cooling; private set => _cooling = value; }
        public string Requirements { get => _requirements; private set => _requirements = value; }
        public bool Valid { get => _valid; private set { _oldValid = _valid; _valid = value; } }
        public bool Active { get => _active; private set { _active = value; } }

        public HeatSinkTypes HeatSinkType { get => _coolerType; private set => _coolerType = value; }


        public HeatSink(string displayName, Bitmap texture, HeatSinkTypes type, double heatPassive, string requirements, Point3D position) : base(displayName, BlockTypes.HeatSink, texture, position)
        {
            HeatSinkType = type;
            Cooling = heatPassive;
            Requirements = requirements;
            Valid = false;
            placementErrors = new List<string>();
        }

        public HeatSink(HeatSink parent, Point3D position) : this(parent.DisplayName, parent.Texture, parent.HeatSinkType, parent.Cooling, parent.Requirements, position)
        {
        }

        public override string GetToolTip()
        {
            string toolTip = DisplayName + " HeatSink\r\n";

            if (Position != Palette.dummyPosition)
            {
                toolTip += string.Format("at: X: {0} Y: {1} Z: {2}\r\n", Position.X, Position.Y, Position.Z);
                toolTip += string.Format(" Cluster: {0}\r\n", Cluster.ToString());
                if(Reactor.state == ReactorStates.Running && Cluster != -1)
                    toolTip += (Reactor.clusters[Cluster].HasPathToCasing ? " Has casing connection\r\n" : " Invalid cluster!\r\n");
            }

            toolTip += string.Format(" Cooling: {0} HU/t\r\n" +
                                    " Requires: {1}\r\n", Cooling, Requirements);
            if (Position != Palette.dummyPosition & !Valid)
            {
                foreach (string error in new HashSet<string>(placementErrors))
                {
                    toolTip += string.Format("    {0}\r\n", error);
                }
            }
            return toolTip;
        }

        public void UpdateStats()
        {
            CheckPlacementValid();
            placementErrors = new HashSet<string>(placementErrors).ToList();
        }

        public override void RevertToSetup()
        {
            SetCluster(-1);
        }

        public override void ReloadValuesFromConfig()
        {
            CoolerValues cv = Configuration.Coolers[HeatSinkType.ToString()];
            Cooling = cv.HeatPassive;
            Requirements = cv.Requirements;
        }

        public bool CheckPlacementValid()
        {
            placementErrors = new List<string>();
            switch (HeatSinkType)
            {
                case HeatSinkTypes.Water:
                    return Valid = HasAdjacent(Palette.blockPalette["FuelCell"]);
                case HeatSinkTypes.Redstone:
                    return Valid = HasAdjacent(Palette.blockPalette["FuelCell"]) & HasAdjacent(Palette.blockPalette["Graphite"]);
                case HeatSinkTypes.Quartz:
                    return Valid = HasAdjacent(Palette.blockPalette["Magnesium"]);
                case HeatSinkTypes.Gold:
                    return Valid = HasAdjacent(Palette.blockPalette["Iron"], 2);
                case HeatSinkTypes.Glowstone:
                    return Valid = HasAdjacent(Palette.blockPalette["Graphite"], 2);
                case HeatSinkTypes.Lapis:
                    return Valid = HasAdjacent(Palette.blockPalette["FuelCell"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case HeatSinkTypes.Diamond:
                    return Valid = HasAdjacent(Palette.blockPalette["Gold"]) & HasAdjacent(Palette.blockPalette["FuelCell"]);
                case HeatSinkTypes.Helium:
                    return Valid = HasAdjacent(Palette.blockPalette["Redstone"], 2, true) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case HeatSinkTypes.Enderium:
                    return Valid = HasAdjacent(Palette.blockPalette["Graphite"], 3);
                case HeatSinkTypes.Cryotheum:
                    return Valid = HasAdjacent(Palette.blockPalette["FuelCell"], 3);
                case HeatSinkTypes.Iron:
                    return Valid = HasAdjacent(Palette.blockPalette["Graphite"]);
                case HeatSinkTypes.Emerald:
                    return Valid = HasAdjacent(Palette.blockPalette["Prismarine"]) & HasAdjacent(Palette.blockPalette["Graphite"]);
                case HeatSinkTypes.Copper:
                    return Valid = HasAdjacent(Palette.blockPalette["Water"]);
                case HeatSinkTypes.Tin:
                    return Valid = HasAdjacent(Palette.blockPalette["Lapis"], 2);
                case HeatSinkTypes.Magnesium:
                    return Valid = HasAdjacent(Palette.blockPalette["Lead"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case HeatSinkTypes.Boron:
                    return Valid = HasAdjacent(Palette.blockPalette["Bronze"]);
                case HeatSinkTypes.Bronze:
                    return Valid = HasAdjacent(Palette.blockPalette["Copper"]) & HasAdjacent(Palette.blockPalette["Tin"]);
                case HeatSinkTypes.Prismarine:
                    return Valid = HasAdjacent(Palette.blockPalette["Water"], 2);
                case HeatSinkTypes.Obsidian:
                    return Valid = HasAdjacent(Palette.blockPalette["Glowstone"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case HeatSinkTypes.Lead:
                    return Valid = HasAdjacent(Palette.blockPalette["Iron"]);
                default:
                    throw new ArgumentException("Unexpected HeatSink type");
            }
        }

        private bool HasAdjacent(Block needed, int number = 1, bool exact = false)
        {
            int adjacent = 0;
            int activeAdjacent = 0;
            foreach (Vector3D o in Reactor.sixAdjOffsets)
            {
                Block block = Reactor.BlockAt(Position + o);
                BlockTypes bt = block.BlockType;
                BlockTypes nt = needed.BlockType;

                //If checked block doesn't match at all: log errors
                //Either cooler types are mismatched or the blocktype is mismatched
                if (((bt == BlockTypes.HeatSink & nt == BlockTypes.HeatSink) && ((HeatSink)block).HeatSinkType != ((HeatSink)needed).HeatSinkType) | bt != nt)
                {
                    if (adjacent == 0)
                        placementErrors.Add("No " + ((nt == BlockTypes.HeatSink) ? ((HeatSink)needed).HeatSinkType.ToString() : nt.ToString()));
                    else if (adjacent < number)
                        placementErrors.Add("Too few " + ((nt == BlockTypes.HeatSink) ? ((HeatSink)needed).HeatSinkType.ToString() : nt.ToString()));

                    continue;
                }

                adjacent++;
                while (placementErrors.Remove("No " + ((nt == BlockTypes.HeatSink) ? ((HeatSink)needed).HeatSinkType.ToString() : nt.ToString())));

                if (adjacent >= number)
                    while (placementErrors.Remove("Too few " + ((nt == BlockTypes.HeatSink) ? ((HeatSink)needed).HeatSinkType.ToString() : nt.ToString())));

                if (block.IsValid())
                {
                    activeAdjacent++;
                    if (activeAdjacent > number & exact)
                        placementErrors.Add("Too many " + ((nt == BlockTypes.HeatSink) ? ((HeatSink)needed).HeatSinkType.ToString() : nt.ToString()));
                }
                else
                    placementErrors.Add("Inactive " + ((nt == BlockTypes.HeatSink) ? ((HeatSink)needed).HeatSinkType.ToString() : nt.ToString()));
            }

            if (exact)
                return activeAdjacent == number;
            else
                return activeAdjacent >= number;
        }

        private bool CheckTin()
        {
            bool hasAxialLapis = false;

            for (int i = 0; i < 3; i++)
            {
                Block block1 = Reactor.BlockAt(Position + Reactor.sixAdjOffsets[2 * i]);
                Block block2 = Reactor.BlockAt(Position + Reactor.sixAdjOffsets[2 * i + 1]);
                if (block1 is HeatSink c1 && c1.HeatSinkType == HeatSinkTypes.Lapis)
                {
                    if (block2 is HeatSink c2 && c2.HeatSinkType == HeatSinkTypes.Lapis)
                    {
                        while (placementErrors.Remove("No axial Lapis")) ;
                        hasAxialLapis = true;
                        if (c1.Valid)
                            if (c2.Valid)
                            {
                                while (placementErrors.Remove("No axial Lapis")) ;
                                return true;
                            }
                            else placementErrors.Add("Inactive Lapis");
                        else placementErrors.Add("Inactive Lapis");
                    }
                    else
                    {
                        placementErrors.Add("No axial Lapis");
                        continue;
                    }
                }
                else
                {
                    if (!hasAxialLapis)
                        placementErrors.Add("No axial Lapis");
                    continue;
                }
            }
            return false;
        }

        private bool CheckEnderium()
        {
            double x = Position.X;
            double y = Position.Y;
            double z = Position.Z;

            if (y != 1 & y != Reactor.interiorDims.Y)
            {
                placementErrors.Add("Not in a corner!");
                return false;
            }

            if(Reactor.interiorDims.Y == 1)
            {
                placementErrors.Add("Pancake reactor, Enderium won't work here!");
                return false;
            }

            if ((x == 1 & z == 1) || (x == 1 & z == Reactor.interiorDims.Z) || (x == Reactor.interiorDims.X & z == 1) || (x == Reactor.interiorDims.X & z == Reactor.interiorDims.Z))
                return true;
            else
                placementErrors.Add("Not in a corner!");

            return false;
        }

        public override bool IsValid()
        {
            return Valid;
        }

        public override Block Copy(Point3D newPosition)
        {
            return new HeatSink(this, newPosition);
        }
    }

    public enum HeatSinkTypes
    {
        Water,
        Redstone,
        Quartz,
        Gold,
        Glowstone,
        Lapis,
        Diamond,
        Helium,
        Enderium,
        Cryotheum,
        Iron,
        Emerald,
        Copper,
        Tin,
        Magnesium,
        Obsidian,
        Prismarine,
        Bronze,
        Boron,
        Lead,

        //NotACooler,
    }
}
