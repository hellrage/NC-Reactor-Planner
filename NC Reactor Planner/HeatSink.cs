using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class HeatSink : Block
    {
        private List<string> placementErrors;
        
        public double Cooling { get ; private set; }
        public string Requirements { get ; private set; }
        public override bool Valid { get; protected set; }

        public HeatSinkTypes HeatSinkType { get ; private set ; }


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
            string toolTip = DisplayName + " heatsink\r\n";

            if (Position != Palette.dummyPosition)
            {
                if (Cluster != -1)
                {
                    toolTip += string.Format("Cluster: {0}\r\n", Cluster);
                    toolTip += (Reactor.clusters[Cluster].HasPathToCasing ? " Has casing connection\r\n" : "--Invalid cluster!\r\n");
                }
                else
                    toolTip += "--No cluster!\r\n";
            }

            toolTip += string.Format(" Cooling: {0} HU/t\r\n" +
                                    " Requires: {1}\r\n", Cooling, Requirements);
            if (Position != Palette.dummyPosition & !Valid)
            {
                foreach (string error in new HashSet<string>(placementErrors))
                {
                    toolTip += string.Format("----{0}\r\n", error);
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
            HeatSinkValues cv = Configuration.HeatSinks[HeatSinkType.ToString()];
            Cooling = cv.HeatPassive;
            Requirements = cv.Requirements;
        }

        public bool CheckPlacementValid()
        {
            placementErrors = new List<string>();
            switch (HeatSinkType)
            {
                case HeatSinkTypes.Water:
                    return Valid = HasAdjacent(Palette.BlockPalette["FuelCell"]);
                case HeatSinkTypes.Redstone:
                    return Valid = HasAdjacent(Palette.BlockPalette["FuelCell"]) & HasAdjacent(Palette.BlockPalette["Graphite"]);
                case HeatSinkTypes.Quartz:
                    return Valid = HasAdjacent(Palette.BlockPalette["Redstone"]);
                case HeatSinkTypes.Gold:
                    return Valid = HasAdjacent(Palette.BlockPalette["Iron"], 2);
                case HeatSinkTypes.Glowstone:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"], 2);
                case HeatSinkTypes.Lapis:
                    return Valid = HasAdjacent(Palette.BlockPalette["FuelCell"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case HeatSinkTypes.Diamond:
                    return Valid = HasAdjacent(Palette.BlockPalette["Gold"]) & HasAdjacent(Palette.BlockPalette["FuelCell"]);
                case HeatSinkTypes.Helium:
                    return Valid = HasAdjacent(Palette.BlockPalette["Redstone"], 2, true) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case HeatSinkTypes.Enderium:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"], 3);
                case HeatSinkTypes.Cryotheum:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"]) & HasAdjacent(Palette.BlockPalette["Reflector"], 2);
                case HeatSinkTypes.Iron:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"]);
                case HeatSinkTypes.Emerald:
                    return Valid = HasAdjacent(Palette.BlockPalette["Prismarine"]) & HasAdjacent(Palette.BlockPalette["Graphite"]);
                case HeatSinkTypes.Copper:
                    return Valid = HasAdjacent(Palette.BlockPalette["Water"]);
                case HeatSinkTypes.Tin:
                    return Valid = HasAxial(Palette.BlockPalette["Lapis"]);
                case HeatSinkTypes.Magnesium:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case HeatSinkTypes.Boron:
                    return Valid = HasAdjacent(Palette.BlockPalette["Quartz"], 1, true) & HasAdjacent(Palette.BlockPalette["Reflector"]);
                case HeatSinkTypes.Prismarine:
                    return Valid = HasAdjacent(Palette.BlockPalette["Water"], 2);
                case HeatSinkTypes.Obsidian:
                    return Valid = HasAxial(Palette.BlockPalette["Glowstone"]);
                case HeatSinkTypes.Lead:
                    return Valid = HasAdjacent(Palette.BlockPalette["Iron"]);
                case HeatSinkTypes.Aluminum:
                    return Valid = HasAdjacent(Palette.BlockPalette["Copper"]) & HasAdjacent(Palette.BlockPalette["Tin"]);
                case HeatSinkTypes.Lithium:
                    return Valid = HasAdjacent(Palette.BlockPalette["Lead"], 1, true) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case HeatSinkTypes.Manganese:
                    return Valid = HasAdjacent(Palette.BlockPalette["FuelCell"], 2);
                case HeatSinkTypes.Silver:
                    return Valid = HasAdjacent(Palette.BlockPalette["Glowstone"]) & HasAdjacent(Palette.BlockPalette["Lapis"]);
                case HeatSinkTypes.Purpur:
                    return Valid = HasAdjacent(Palette.BlockPalette["Obsidian"]);
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
                //Either heatsink types are mismatched or the blocktype is mismatched
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

                if (block.Valid)
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

        private bool HasAxial(Block needed)
        {
            bool hasAxial = false;
            BlockTypes bn = needed.BlockType;

            for (int i = 0; i < 3; i++)
            {
                Block block1 = Reactor.BlockAt(Position + Reactor.sixAdjOffsets[2 * i]);
                BlockTypes bt1 = block1.BlockType;
                Block block2 = Reactor.BlockAt(Position + Reactor.sixAdjOffsets[2 * i + 1]);
                BlockTypes bt2 = block2.BlockType;

                if (bt1 == bn)
                {
                    if (bt2 == bn)
                    {
                        if ((needed is HeatSink hs && (((HeatSink)block1).HeatSinkType == ((HeatSink)block2).HeatSinkType)))
                            if (((HeatSink)block1).HeatSinkType == hs.HeatSinkType)
                            {
                                while (placementErrors.Remove("No axial " + needed.DisplayName)) ;
                                hasAxial = true;
                                if (block1.Valid)
                                    if (block2.Valid)
                                    {
                                        while (placementErrors.Remove("No axial " + needed.DisplayName)) ;
                                        return true;
                                    }
                                    else placementErrors.Add("Inactive " + needed.DisplayName);
                                else placementErrors.Add("Inactive " + needed.DisplayName);
                            }
                    }
                    else
                    {
                        placementErrors.Add("No axial " + needed.DisplayName);
                        continue;
                    }
                }
                else
                {
                    if (!hasAxial)
                        placementErrors.Add("No axial " + needed.DisplayName);
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
        Boron,
        Lead,
        Purpur,
        Manganese,
        Aluminum,
        Silver,
        Lithium,
        //NotACooler,
    }
}
