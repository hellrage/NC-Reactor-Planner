﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class Cooler : Block
    {
        private bool _oldValid;
        private bool _valid;
        private List<string> placementErrors;

        private CoolerTypes _coolerType;

        public double HeatActive { get; private set; }
        public double HeatPassive { get; private set; }
        public double Cooling { get => Active ? HeatActive * Configuration.Fission.ActiveCoolerMaxRate / 20 : HeatPassive; }
        public string Requirements { get; private set; }
        public override bool Valid { get => _valid; protected set { _oldValid = _valid; _valid = value; } }
        public bool Active { get; private set; }

        public CoolerTypes CoolerType { get => _coolerType; private set => _coolerType = value; }


        public Cooler(string displayName, Bitmap texture, CoolerTypes type, double heatActive, double heatPassive, string requirements, Point3D position, bool active = false) : base(displayName, BlockTypes.Cooler, texture, position)
        {
            CoolerType = type;
            HeatActive = heatActive;
            HeatPassive = heatPassive;
            Requirements = requirements;
            Valid = false;
            Active = active;
            placementErrors = new List<string>();
        }

        public Cooler(Cooler parent, Point3D position) : this(parent.DisplayName, parent.Texture, parent.CoolerType, parent.HeatActive, parent.HeatPassive, parent.Requirements, position, parent.Active)
        {
        }

        public Cooler(Cooler parent, Point3D position, bool active) : this(parent.DisplayName, parent.Texture, parent.CoolerType, parent.HeatActive, parent.HeatPassive, parent.Requirements, position, active)
        {
        }

        public override string GetToolTip()
        {
            string toolTip = (Active ? "Active " : "") + DisplayName + " Cooler\r\n";

            if (Position != Palette.dummyPosition)
                toolTip += string.Format("at: X: {0} Y: {1} Z: {2}\r\n", Position.X, Position.Y, Position.Z);

            toolTip += string.Format(" Passive cooling: {0} HU/t\r\n" +
                                    " Active cooling: {1} HU/t\r\n" +
                                    " Requires: {2}\r\n", HeatPassive, HeatActive * Configuration.Fission.ActiveCoolerMaxRate / 20, Requirements);
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

        public override void ReloadValuesFromConfig()
        {
            CoolerValues cv = Configuration.Coolers[CoolerType.ToString()];
            HeatActive = cv.HeatActive;
            HeatPassive = cv.HeatPassive;
            Requirements = cv.Requirements;
        }

        public bool CheckPlacementValid()
        {
            placementErrors = new List<string>();
            switch (CoolerType)
            {
                case CoolerTypes.Water:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"]) | HasAdjacent(Palette.BlockPalette["FuelCell"]);
                case CoolerTypes.Redstone:
                    return Valid = HasAdjacent(Palette.BlockPalette["FuelCell"]);
                case CoolerTypes.Quartz:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"]);
                case CoolerTypes.Gold:
                    return Valid = HasAdjacent(Palette.BlockPalette["Water"]) & HasAdjacent(Palette.BlockPalette["Redstone"]);
                case CoolerTypes.Glowstone:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"], 2);
                case CoolerTypes.Lapis:
                    return Valid = HasAdjacent(Palette.BlockPalette["FuelCell"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case CoolerTypes.Diamond:
                    return Valid = HasAdjacent(Palette.BlockPalette["Water"]) & HasAdjacent(Palette.BlockPalette["Quartz"]);
                case CoolerTypes.Helium:
                    return Valid = HasAdjacent(Palette.BlockPalette["Redstone"], 1, true) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case CoolerTypes.Enderium:
                    return Valid = CheckEnderium();
                case CoolerTypes.Cryotheum:
                    return Valid = HasAdjacent(Palette.BlockPalette["FuelCell"], 2);
                case CoolerTypes.Iron:
                    return Valid = HasAdjacent(Palette.BlockPalette["Gold"]);
                case CoolerTypes.Emerald:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"]) & HasAdjacent(Palette.BlockPalette["FuelCell"]);
                case CoolerTypes.Copper:
                    return Valid = HasAdjacent(Palette.BlockPalette["Glowstone"]);
                case CoolerTypes.Tin:
                    return Valid = CheckTin();
                case CoolerTypes.Magnesium:
                    return Valid = HasAdjacent(Palette.BlockPalette["Graphite"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                default:
                    throw new ArgumentException("Unexpected cooler type");
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
                if (((bt == BlockTypes.Cooler & nt == BlockTypes.Cooler) && ((Cooler)block).CoolerType != ((Cooler)needed).CoolerType) | bt != nt)
                {
                    if (adjacent == 0)
                        placementErrors.Add("No " + ((nt == BlockTypes.Cooler) ? ((Cooler)needed).CoolerType.ToString() : nt.ToString()));
                    else if (adjacent < number)
                        placementErrors.Add("Too few " + ((nt == BlockTypes.Cooler) ? ((Cooler)needed).CoolerType.ToString() : nt.ToString()));

                    continue;
                }

                adjacent++;
                while (placementErrors.Remove("No " + ((nt == BlockTypes.Cooler) ? ((Cooler)needed).CoolerType.ToString() : nt.ToString())));

                if (adjacent >= number)
                    while (placementErrors.Remove("Too few " + ((nt == BlockTypes.Cooler) ? ((Cooler)needed).CoolerType.ToString() : nt.ToString())));

                if (block.Valid)
                {
                    activeAdjacent++;
                    if (activeAdjacent > number & exact)
                        placementErrors.Add("Too many " + ((nt == BlockTypes.Cooler) ? ((Cooler)needed).CoolerType.ToString() : nt.ToString()));
                }
                else
                    placementErrors.Add("Inactive " + ((nt == BlockTypes.Cooler) ? ((Cooler)needed).CoolerType.ToString() : nt.ToString()));
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
                if (block1 is Cooler c1 && c1.CoolerType == CoolerTypes.Lapis)
                {
                    if (block2 is Cooler c2 && c2.CoolerType == CoolerTypes.Lapis)
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

        public override Block Copy(Point3D newPosition)
        {
            return new Cooler(this, newPosition);
        }
    }

    public enum CoolerTypes
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
        //NotACooler,
    }
}
