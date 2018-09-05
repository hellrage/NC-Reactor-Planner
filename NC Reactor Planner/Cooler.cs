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
    public class Cooler : Block
    {
        private int _heatActive;
        private int _heatPassive;
        private string _requirements;
        private bool _active;
        private List<CoolerErrorTypes> _placementErrors;
        private List<string> _sPlacementErrors;

        private CoolerTypes _coolerType;

        public int HeatActive { get => _heatActive; private set => _heatActive = value; }
        public int HeatPassive { get => _heatPassive; private set => _heatPassive = value; }
        public string Requirements { get => _requirements; private set => _requirements = value; }
        public bool Active { get => _active; private set => _active = value; }
        public List<CoolerErrorTypes> PlacementErrors { get => _placementErrors; private set => _placementErrors = value; }

        public CoolerTypes CoolerType { get => _coolerType; private set => _coolerType = value; }


        public Cooler(string displayName, Bitmap texture, CoolerTypes type, int heatActive, int heatPassive, string requirements, Point3D position) : base(displayName, BlockTypes.Cooler, texture, position)
        {
            CoolerType = type;
            HeatActive = heatActive;
            HeatPassive = heatPassive;
            Requirements = requirements;
            Active = false;
            PlacementErrors = new List<CoolerErrorTypes>();
        }

        public Cooler(Cooler parent, Point3D position) : this(parent.DisplayName, parent.Texture, parent.CoolerType, parent.HeatActive, parent.HeatPassive, parent.Requirements, position)
        {
        }

        public override string GetToolTip()
        {
            string toolTip = DisplayName + " cooler\r\n";
            if (Position != Palette.dummyPosition)
                toolTip += string.Format("at: X: {0} Y: {1} Z: {2}\r\n", Position.X, Position.Y, Position.Z);
            toolTip += string.Format(" Passive cooling: {0} HU/t\r\n" +
                                    " Active cooling: {1} HU/t\r\n" +
                                    " Requires: {2}\r\n", HeatPassive, HeatActive, Requirements);
            if (Position != Palette.dummyPosition & !Active)
            {
                //foreach (CoolerErrorTypes error in new HashSet<CoolerErrorTypes>(PlacementErrors))
                //{
                //    toolTip += string.Format("    {0}\r\n", error.ToString());
                //}
                foreach (string error in new HashSet<string>(_sPlacementErrors))
                {
                    toolTip += string.Format("    {0}\r\n", error);
                }
            }
            return toolTip;
        }

        public void UpdateStats()
        {
            //CheckPlacementValid();
            NewCheckPlacementValid();
            //PlacementErrors = new HashSet<CoolerErrorTypes>(PlacementErrors).ToList();
            _sPlacementErrors = new HashSet<string>(_sPlacementErrors).ToList();
        }

        public override void ReloadValuesFromSetttings()
        {
            List<string> values = ModValueSettings.RetrieveSplitSettings(DisplayName);
            try
            {
                HeatPassive = Convert.ToInt32(values[0]);
                HeatActive = Convert.ToInt32(values[1]);
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + string.Format("\r\n There were invalid values in the settings: {0}: {1} {2}\r\n Leaving the values as default.", DisplayName, values[0], values[1]));
            }
        }

        public bool CheckPlacementValid()
        {
            Active = false;
            PlacementErrors = new List<CoolerErrorTypes>();
            _sPlacementErrors = new List<string>();

            bool hasRedstone = false;
            bool hasActiveRedstone = false;
            bool hasWater = false;
            bool hasActiveWater = false;
            bool hasCell = false;
            bool hasCasing = false;
            bool hasAxialLapis = false;
            bool hasGold = false;
            bool hasGlowstone = false;
            bool hasQuartz = false;
            bool hasActiveQuartz = false;
            bool hasModerator = false;
            bool hasActiveModerator = false;
            int activeModerators = 0;
            int waterCoolers = 0;
            int activeWaterCoolers = 0;
            int activeRedstoneCoolers = 0;
            int casings = 0;
            int cells = 0;
            int moderators = 0;

            switch (CoolerType)
            {
                case CoolerTypes.Water:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is FuelCell)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoCell)) ;
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoModerator)) ;
                            while (PlacementErrors.Remove(CoolerErrorTypes.InactiveModerator)) ;
                            hasCell = true;
                            return Active = true;
                        }
                        else PlacementErrors.Add(CoolerErrorTypes.NoCell);

                        if (block is Moderator moderator)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoModerator)) ;
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoCell)) ;
                            hasModerator = true;
                            if (moderator.Active)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.InactiveModerator)) ;
                                hasActiveModerator = true;
                                return Active = true;
                            }
                            else if (!hasCell) PlacementErrors.Add(CoolerErrorTypes.InactiveModerator);
                        }
                        else if (!hasCell & !hasModerator) PlacementErrors.Add(CoolerErrorTypes.NoModerator);

                    }
                    return Active;
                case CoolerTypes.Redstone:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is FuelCell)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoCell)) ;
                            return Active = true;
                        }
                        else PlacementErrors.Add(CoolerErrorTypes.NoCell);
                    }
                    return Active;
                case CoolerTypes.Quartz:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Moderator moderator)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoModerator)) ;
                            hasModerator = true;
                            if (moderator.Active)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.InactiveModerator)) ;
                                return Active = true;
                            }
                            else PlacementErrors.Add(CoolerErrorTypes.InactiveModerator);
                        }
                        else if(!hasModerator) PlacementErrors.Add(CoolerErrorTypes.NoModerator);
                    }
                    return Active;
                case CoolerTypes.Gold:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Cooler cooler)
                        {
                            if (cooler.CoolerType == CoolerTypes.Water)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.NoWater)) ;
                                hasWater = true;
                                if (cooler.Active)
                                {
                                    while (PlacementErrors.Remove(CoolerErrorTypes.InactiveWater)) ;
                                    hasActiveWater = true;
                                    if (hasActiveRedstone) return Active = true;
                                }
                                else PlacementErrors.Add(CoolerErrorTypes.InactiveWater);
                            }

                            if (cooler.CoolerType == CoolerTypes.Redstone)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.NoRedstone)) ;
                                hasRedstone = true;
                                if (cooler.Active)
                                {
                                    while (PlacementErrors.Remove(CoolerErrorTypes.InactiveRedstone)) ;
                                    hasActiveRedstone = true;
                                    if (hasActiveWater) return Active = true;
                                }
                                else PlacementErrors.Add(CoolerErrorTypes.InactiveRedstone);
                            }
                        }
                        else
                        {
                            if (!hasRedstone) PlacementErrors.Add(CoolerErrorTypes.NoRedstone);
                            if (!hasWater) PlacementErrors.Add(CoolerErrorTypes.NoWater);
                        }
                    }
                    return Active;
                case CoolerTypes.Glowstone:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Moderator moderator)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoModerator)) ;
                            moderators++;
                            if (moderators >= 2) while (PlacementErrors.Remove(CoolerErrorTypes.TooFewModerators)) ;
                            if (moderator.Active)
                            {
                                activeModerators++;
                                if(activeModerators >=2) while (PlacementErrors.Remove(CoolerErrorTypes.InactiveModerator)) ;
                                if (activeModerators >= 2) return Active = true;
                            }
                            else if(activeModerators < 2) PlacementErrors.Add(CoolerErrorTypes.InactiveModerator);
                        }
                        else if (moderators < 2) PlacementErrors.Add(CoolerErrorTypes.TooFewModerators);
                    }
                    Active = activeModerators >= 2;
                    return Active;
                case CoolerTypes.Lapis:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is FuelCell)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoCell)) ;
                            hasCell = true;
                            if (hasCasing) return Active = true;
                        }
                        else if(!hasCell) PlacementErrors.Add(CoolerErrorTypes.NoCell);

                        if (block is Casing)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoCasing)) ;
                            hasCasing = true;
                            if (hasCell)return Active = true;
                        }
                        else if(!hasCasing) PlacementErrors.Add(CoolerErrorTypes.NoCasing);
                    }
                    return Active;
                case CoolerTypes.Diamond:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Cooler cooler)
                        {
                            if (cooler.CoolerType == CoolerTypes.Water)
                            {
                                waterCoolers++;
                                if (waterCoolers >= 2) while (PlacementErrors.Remove(CoolerErrorTypes.TooFewWater)) ;
                                if (cooler.Active)
                                {
                                    activeWaterCoolers++;
                                    if (activeWaterCoolers >=2 ) while (PlacementErrors.Remove(CoolerErrorTypes.InactiveWater)) ;
                                    if (hasActiveQuartz & activeWaterCoolers >= 2)
                                    {
                                        return Active = true;
                                    }
                                }
                                else if(activeWaterCoolers < 2) PlacementErrors.Add(CoolerErrorTypes.InactiveWater);
                            }
                            else if (waterCoolers < 2) PlacementErrors.Add(CoolerErrorTypes.TooFewWater);

                            if (cooler.CoolerType == CoolerTypes.Quartz)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.NoQuartz)) ;
                                hasQuartz = true;
                                if (cooler.Active)
                                {
                                    while (PlacementErrors.Remove(CoolerErrorTypes.InactiveQuartz)) ;
                                    hasActiveQuartz = true;
                                    if (activeWaterCoolers >= 2)return Active = true;
                                }
                                else PlacementErrors.Add(CoolerErrorTypes.InactiveQuartz);
                            }
                            else if (!hasQuartz) PlacementErrors.Add(CoolerErrorTypes.NoQuartz);
                        }
                        else
                        {
                            if (!hasQuartz) PlacementErrors.Add(CoolerErrorTypes.NoQuartz);
                            if (waterCoolers < 2) PlacementErrors.Add(CoolerErrorTypes.TooFewWater);
                        }
                    }
                    return Active;
                case CoolerTypes.Helium:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Cooler cooler)
                        {
                            if (cooler.CoolerType == CoolerTypes.Redstone)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.NoRedstone)) ;
                                hasRedstone = true;
                                if (cooler.Active)
                                {
                                    activeRedstoneCoolers++;
                                    while (PlacementErrors.Remove(CoolerErrorTypes.InactiveRedstone)) ;
                                    if (hasCasing & activeRedstoneCoolers == 1) return Active = true;
                                    if (activeRedstoneCoolers > 1)
                                    {
                                        PlacementErrors.Add(CoolerErrorTypes.TooManyRedstone);
                                        return Active = false;
                                    }
                                }
                                else if (!hasActiveRedstone) PlacementErrors.Add(CoolerErrorTypes.InactiveRedstone);
                            }
                        }
                        else
                        {
                            if (!hasRedstone) PlacementErrors.Add(CoolerErrorTypes.NoRedstone);
                            if (!hasCasing) PlacementErrors.Add(CoolerErrorTypes.NoCasing);
                        }

                        if (block is Casing)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoCasing)) ;
                            hasCasing = true;
                            if (hasCasing & activeRedstoneCoolers == 1) return Active = true;
                        }

                        
                    }
                    return Active;
                case CoolerTypes.Enderium:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Casing)
                            casings++;
                    }
                    if (casings == 0) PlacementErrors.Add(CoolerErrorTypes.NoCasing);
                    if (casings < 3) PlacementErrors.Add(CoolerErrorTypes.TooFewCasings);
                    return Active = casings >= 3;
                case CoolerTypes.Cryotheum:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is FuelCell)
                            cells++;
                    }
                    if (cells == 0) PlacementErrors.Add(CoolerErrorTypes.NoCell);
                    if (cells < 2) PlacementErrors.Add(CoolerErrorTypes.TooFewCells);
                    return Active = cells >= 2;
                case CoolerTypes.Iron:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Cooler cooler && cooler.CoolerType == CoolerTypes.Gold)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoGold)) ;
                            hasGold = true;
                            if(cooler.Active)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.InactiveGold)) ;
                                return Active = true;
                            }
                            else PlacementErrors.Add(CoolerErrorTypes.InactiveGold);
                        }
                        else if(!hasGold) PlacementErrors.Add(CoolerErrorTypes.NoGold);
                    }
                    return Active;
                case CoolerTypes.Emerald:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Moderator moderator)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoModerator)) ;
                            hasModerator = true;
                            if (moderator.Active)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.InactiveModerator)) ;
                                hasActiveModerator = true;
                                if (hasCell) return Active = true;
                            }
                            else if(!hasActiveModerator) PlacementErrors.Add(CoolerErrorTypes.InactiveModerator);
                        }
                        else if (!hasModerator) PlacementErrors.Add(CoolerErrorTypes.NoModerator);

                        if (block is FuelCell)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoCell)) ;
                            hasCell = true;
                            if (hasActiveModerator) return Active = true;
                        }
                        else if(!hasCell) PlacementErrors.Add(CoolerErrorTypes.NoCell);
                    }
                    return Active;
                case CoolerTypes.Copper:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Cooler cooler && cooler.CoolerType == CoolerTypes.Glowstone)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoGlowstone)) ;
                            hasGlowstone = true;
                            if (cooler.Active)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.InactiveGlowstone)) ;
                                return Active = true;
                            }
                            else PlacementErrors.Add(CoolerErrorTypes.InactiveGlowstone);
                        }
                        else if(!hasGlowstone) PlacementErrors.Add(CoolerErrorTypes.NoGlowstone);
                    }
                    return Active;
                case CoolerTypes.Tin:
                    for (int i = 0; i < 3; i++)
                    {
                        Block block1 = Reactor.BlockAt(Position + Reactor.sixAdjOffsets[2 * i]);
                        Block block2 = Reactor.BlockAt(Position + Reactor.sixAdjOffsets[2 * i + 1]);
                        if (block1 is Cooler c1 && c1.CoolerType == CoolerTypes.Lapis)
                        {
                            if (block2 is Cooler c2 && c2.CoolerType == CoolerTypes.Lapis)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.NoAxialLapis)) ;
                                hasAxialLapis = true;
                                if (c1.Active)
                                {
                                    if (c2.Active)
                                    {
                                        while (PlacementErrors.Remove(CoolerErrorTypes.InactiveLapis)) ;
                                        return Active = true;
                                    }
                                    else PlacementErrors.Add(CoolerErrorTypes.InactiveLapis);
                                }
                                else PlacementErrors.Add(CoolerErrorTypes.InactiveLapis);
                            }
                            else
                            {
                                PlacementErrors.Add(CoolerErrorTypes.NoAxialLapis);
                                continue;
                            }
                        }
                        else
                        {
                            if(!hasAxialLapis)
                                PlacementErrors.Add(CoolerErrorTypes.NoAxialLapis);
                            continue;
                        }
                    }
                    return Active;
                case CoolerTypes.Magnesium:
                    foreach (Vector3D o in Reactor.sixAdjOffsets)
                    {
                        Block block = Reactor.BlockAt(Position + o);
                        if (block is Moderator moderator)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoModerator)) ;
                            hasModerator = true;
                            if (moderator.Active)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.InactiveModerator)) ;
                                hasActiveModerator = true;
                                if (hasCasing) return Active = true;
                            }
                            else if (!hasActiveModerator) PlacementErrors.Add(CoolerErrorTypes.InactiveModerator);
                        }
                        else if(!hasModerator) PlacementErrors.Add(CoolerErrorTypes.NoModerator);

                        if (block is Casing)
                        {
                            while (PlacementErrors.Remove(CoolerErrorTypes.NoCasing)) ;
                            hasCasing = true;
                            if (hasActiveModerator) return Active = true;
                        }
                        else if (!hasCasing) PlacementErrors.Add(CoolerErrorTypes.NoCasing);
                    }
                    return Active;
                default:
                    throw new ArgumentException();
            }
        }

        public bool NewCheckPlacementValid()
        {
            _sPlacementErrors = new List<string>();
            switch (CoolerType)
            {
                case CoolerTypes.Water:
                    return Active = HasAdjacent(Palette.blockPalette["Graphite"]) | HasAdjacent(Palette.blockPalette["FuelCell"]);
                case CoolerTypes.Redstone:
                    return Active = HasAdjacent(Palette.blockPalette["FuelCell"]);
                case CoolerTypes.Quartz:
                    return Active = HasAdjacent(Palette.blockPalette["Graphite"]);
                case CoolerTypes.Gold:
                    return Active = HasAdjacent(Palette.blockPalette["Water"]) & HasAdjacent(Palette.blockPalette["Redstone"]);
                case CoolerTypes.Glowstone:
                    return Active = HasAdjacent(Palette.blockPalette["Graphite"], 2);
                case CoolerTypes.Lapis:
                    return Active = HasAdjacent(Palette.blockPalette["FuelCell"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case CoolerTypes.Diamond:
                    return Active = HasAdjacent(Palette.blockPalette["Water"]) & HasAdjacent(Palette.blockPalette["Quartz"]);
                case CoolerTypes.Helium:
                    return Active = HasAdjacent(Palette.blockPalette["Redstone"], 1, true) & HasAdjacent(new Casing("Casing", null, new Point3D()));
                case CoolerTypes.Enderium:
                    return Active = HasAdjacent(new Casing("Casing", null, new Point3D()), 3);
                case CoolerTypes.Cryotheum:
                    return Active = HasAdjacent(Palette.blockPalette["FuelCell"], 2);
                case CoolerTypes.Iron:
                    return Active = HasAdjacent(Palette.blockPalette["Gold"]);
                case CoolerTypes.Emerald:
                    return Active = HasAdjacent(Palette.blockPalette["Graphite"]) & HasAdjacent(Palette.blockPalette["FuelCell"]);
                case CoolerTypes.Copper:
                    return Active = HasAdjacent(Palette.blockPalette["Glowstone"]);
                case CoolerTypes.Tin:
                    return Active = CheckTin();
                case CoolerTypes.Magnesium:
                    return Active = HasAdjacent(Palette.blockPalette["Graphite"]) & HasAdjacent(new Casing("Casing", null, new Point3D()));
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
                if (((block.BlockType == BlockTypes.Cooler & needed.BlockType == BlockTypes.Cooler) && ((Cooler)block).CoolerType != ((Cooler)needed).CoolerType) | block.BlockType != needed.BlockType)
                {
                    if (adjacent == 0)
                    {
                        _sPlacementErrors.Add("No " + ((nt == BlockTypes.Cooler) ? ((Cooler)needed).CoolerType.ToString() : nt.ToString()));
                    }
                    else if (adjacent < number)
                    {
                        _sPlacementErrors.Add("Too few " + ((nt == BlockTypes.Cooler) ? ((Cooler)needed).CoolerType.ToString() : nt.ToString()));
                    }

                    continue;
                }

                //If checked block matched
                if (bt == BlockTypes.FuelCell | bt == BlockTypes.Casing)
                {
                    activeAdjacent++;
                    adjacent++;
                    while (_sPlacementErrors.Remove("No " + bt.ToString())) ;
                    if (activeAdjacent > number & exact)
                    {
                        _sPlacementErrors.Add("Too many " + bt.ToString() + "s");
                        //return false;
                    }
                    //else if (activeAdjacent == number)
                    //    return true;
                }
                else if(bt == BlockTypes.Moderator)
                {
                    Moderator moderator = block as Moderator;
                    if (++adjacent >= number)
                    {
                        while (_sPlacementErrors.Remove("No Moderator")) ;
                        while (_sPlacementErrors.Remove("Too few Moderator")) ;
                    }
                    if(moderator.Active)
                    {
                        activeAdjacent++;
                        if (activeAdjacent >= number & exact)
                        {
                            _sPlacementErrors.Add("Too many Moderators");
                            //return false;
                        }
                        //else if (activeAdjacent == number)
                        //    return true;
                    }
                    else
                        _sPlacementErrors.Add("Inactive Moderator");
                }
                else if(bt == BlockTypes.Cooler)
                {
                    Cooler cooler = block as Cooler;
                    if (++adjacent >= number)
                    {
                        while (_sPlacementErrors.Remove("No " + cooler.CoolerType.ToString())) ;
                        while (_sPlacementErrors.Remove("Too few " + cooler.CoolerType.ToString())) ;
                    }
                    if(cooler.Active)
                    {
                        activeAdjacent++;
                        if (activeAdjacent > number & exact)
                        {
                            _sPlacementErrors.Add("Too many " + cooler.CoolerType);
                            //return false;
                        }
                        //else if (activeAdjacent == number)
                        //    return true;
                    }
                    else
                        _sPlacementErrors.Add("Inactive " + cooler.CoolerType.ToString());
                }
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
                        while (PlacementErrors.Remove(CoolerErrorTypes.NoAxialLapis)) ;
                        hasAxialLapis = true;
                        if (c1.Active)
                        {
                            if (c2.Active)
                            {
                                while (PlacementErrors.Remove(CoolerErrorTypes.InactiveLapis)) ;
                                return true;
                            }
                            else PlacementErrors.Add(CoolerErrorTypes.InactiveLapis);
                        }
                        else PlacementErrors.Add(CoolerErrorTypes.InactiveLapis);
                    }
                    else
                    {
                        PlacementErrors.Add(CoolerErrorTypes.NoAxialLapis);
                        continue;
                    }
                }
                else
                {
                    if (!hasAxialLapis)
                        PlacementErrors.Add(CoolerErrorTypes.NoAxialLapis);
                    continue;
                }
            }
            return false;
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

    public enum CoolerErrorTypes
    {
        NoCell,
        NoCasing,
        NoModerator,
        NoWater,
        NoRedstone,
        NoQuartz,
        NoGold,
        NoGlowstone,
        NoLapis,
        NoAxialLapis,
        InactiveModerator,
        InactiveWater,
        InactiveRedstone,
        InactiveQuartz,
        InactiveLapis,
        InactiveGold,
        InactiveGlowstone,
        TooFewModerators,
        TooFewWater,
        TooFewLapis,
        TooFewCells,
        TooFewCasings,
        TooManyRedstone
    }
}
