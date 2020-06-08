using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;
using System.Text;

namespace NC_Reactor_Planner
{
    public class FuelCell : Block
    {
        public double HeatProducedPerTick { get => HeatMultiplier * UsedFuel.BaseHeat;}
        public double HeatMultiplier { get => AdjacentModeratorLines; }
        public List<FuelCell> AdjacentCells { get; private set; }
        public List<Reflector> AdjacentReflectors { get; private set; }
        public List<Irradiator> AdjacentIrradiators { get; private set; }
        public int AdjacentModeratorLines { get; set; }
        public bool Active { get; private set; }
        public override bool Valid { get => Active; }
        public override bool ReducesSparsity => Active;
        public bool Shielded { get; private set; }
        public Fuel UsedFuel { get; private set; }
        public double PositionalEfficiency { get; set; }
        public int ModeratedNeutronFlux { get; private set; }
        public double Efficiency { get => CalculateEfficiency(); }
        public double FuelDuration { get => UsedFuel.FuelTime * Reactor.clusters[Cluster].FuelDurationMultiplier / Configuration.Fission.FuelUse; }
        public bool Primed { get; set; }
        public string NeutronSource { get; private set; }

        public FuelCell(string displayName, Bitmap texture, Vector3 position, Fuel usedFuel, bool primed = false, string neutronSource = "None") : base(displayName, BlockTypes.FuelCell, texture, position)
        {
            UsedFuel = usedFuel;
            if(UsedFuel.SelfPriming)
            {
                Primed = true;
                NeutronSource = "Self";
            }
            else
            {
                if(neutronSource == "Self")
                {
                    NeutronSource = "None";
                    Primed = false;
                }
                else
                {
                    Primed = primed;
                    NeutronSource = neutronSource;
                }
            }
            RevertToSetup();
        }

        public override void RevertToSetup()
        {
            SetCluster(-1);
            AdjacentCells = new List<FuelCell>();
            AdjacentReflectors = new List<Reflector>();
            AdjacentIrradiators = new List<Irradiator>();
            AdjacentModeratorLines = 0;
            PositionalEfficiency = 0;
            ModeratedNeutronFlux = 0;
            Active = false;
        }

        public FuelCell(FuelCell parent, Vector3 position, Fuel usedFuel, bool primed = false) : this(parent.DisplayName, parent.Texture, position, usedFuel, primed, parent.NeutronSource)
        {
        }

        public override string GetToolTip()
        {
            StringBuilder tb = new StringBuilder(); //TooltipBuilder
            tb.AppendLine(DisplayName);
            if (Position != Palette.dummyPosition)
            {
                tb.Append(base.GetToolTip());
                tb.AppendLine($" Fuel : {UsedFuel.Name}");
                tb.AppendLine(Active ? " Active" : "--Inactive!");
                tb.AppendLine($" Adjacent cells: {AdjacentCells.Count}");
#if DEBUG
                foreach (FuelCell fc in AdjacentCells)
                    tb.AppendLine($"   {fc.Position.ToString()}");
#endif
                tb.AppendLine($" Adjacent moderator lines: {AdjacentModeratorLines}");
                tb.AppendLine($" Adjacent reflectors: {AdjacentReflectors.Count}");
                tb.AppendLine($" Heat multiplier: {(int)(HeatMultiplier * 100)} %");
                tb.AppendLine($" Heat produced: {HeatProducedPerTick} HU/t");
                tb.AppendLine($" Efficiency: {(int)(Efficiency * 100)} %");
                tb.AppendLine($" Positional Eff.: {(int)(PositionalEfficiency * 100)} %");
                tb.AppendLine($" Total Neutron Flux: {ModeratedNeutronFlux}");
                tb.AppendLine($" Criticality factor: {UsedFuel.CriticalityFactor}");
                if (UsedFuel.SelfPriming)
                    tb.AppendLine("Self-priming!");
                if (Primed)
                {
                    tb.AppendLine("Primed");
                    tb.AppendLine($"Neutron source: {NeutronSource}");
                }
            }
            return tb.ToString();
        }

        public List<FuelCell> FindModeratorsThenAdjacentCells()
        {
            List<FuelCell> moderatorAdjacentCells = new List<FuelCell>();
            FuelCell fuelCell;

            foreach (Vector3 offset in Reactor.sixAdjOffsets)
                if ((fuelCell = FindModeratorThenAdjacentCell(offset)) != null)
                    if (!moderatorAdjacentCells.Contains(fuelCell))
                        moderatorAdjacentCells.Add(fuelCell);

            return moderatorAdjacentCells;
        }

        public FuelCell FindModeratorThenAdjacentCell(Vector3 offset)
        {
            int sumModeratorFlux = 0;
            double sumModeratorEfficiency = 0;
            int moderatorsInLine = 0;
            Vector3 pos = Position + offset;
            int i = 1;
            while (Reactor.PositionInsideInterior(pos) && i <= Configuration.Fission.NeutronReach + 1)
            {
                Block block = Reactor.BlockAt(pos);
                if (block is Moderator moderator)
                {
                    sumModeratorFlux += moderator.FluxFactor;
                    sumModeratorEfficiency += moderator.EfficiencyFactor;
                    moderatorsInLine++;
                    i++;
                    pos = Position + offset * i;
                    continue;
                }
                else if (block is FuelCell fuelCell && i > 1 && !fuelCell.AdjacentCells.Contains(this))
                {
                    fuelCell.PositionalEfficiency += sumModeratorEfficiency / moderatorsInLine;
                    fuelCell.ModeratedNeutronFlux += sumModeratorFlux;
                    fuelCell.AddAdjacentFuelCell(this);
                    if (fuelCell.ModeratedNeutronFlux >= fuelCell.UsedFuel.CriticalityFactor || fuelCell.Active)
                    {
                        fuelCell.Activate();
                        return fuelCell;
                    }
                    else
                        return null;
                }
                else if (block is Reflector reflector)
                {
                    if (AdjacentReflectors.Contains(reflector))
                    {
                        if (Active)
                            reflector.Active = true;
                        return null;
                    }
                    if (i < 2 || i > (Configuration.Fission.NeutronReach / 2) + 1)
                        return null;
                    ModeratedNeutronFlux += (int)(2 * sumModeratorFlux * reflector.ReflectivityMultiplier);
                    PositionalEfficiency += reflector.EfficiencyMultiplier * sumModeratorEfficiency / moderatorsInLine;
                    reflector.AddAdjacentFuelCell(this);
                    if (ModeratedNeutronFlux >= UsedFuel.CriticalityFactor)
                    {
                        Activate();
                        reflector.Active = true;
                    }
                    AdjacentReflectors.Add(reflector);
                    return null;
                }
                else if(block is NeutronShield neutronShield)
                {
                    if (neutronShield.Active)
                        return null;
                    sumModeratorEfficiency += neutronShield.EfficiencyFactor;
                    moderatorsInLine++;
                    i++;
                    pos = Position + offset * i;
                    continue;
                }
                else if (block is Irradiator irradiator && !AdjacentIrradiators.Contains(irradiator) && moderatorsInLine >= 1)
                {
                    AdjacentIrradiators.Add(irradiator);
                    return null;
                }
                else
                    return null;

            }
            return null;
        }

        public void AddAdjacentFuelCell(FuelCell fuelCell)
        {
            AdjacentCells.Add(fuelCell);
        }

        public void FilterAdjacentStuff()
        {
            if(!Active)
            {
                AdjacentCells.Clear();
                AdjacentModeratorLines = 0;
                return;
            }
            AdjacentCells = AdjacentCells.FindAll(fc => fc.Active);
            AdjacentIrradiators = AdjacentIrradiators.FindAll(ir => ir.Valid);
            AdjacentModeratorLines = AdjacentCells.Count + AdjacentReflectors.Count + AdjacentIrradiators.Count;
        }

        public bool CanBePrimed()
        {
            if (UsedFuel.SelfPriming)
                return true;
            bool primable;
            foreach (Vector3 offset in Reactor.sixAdjOffsets)
            {
                int i = 1;
                primable = true;
                Vector3 pos = Position + i * offset;
                while (Reactor.PositionInsideInterior(pos))
                {
                    BlockTypes blockType = Reactor.BlockAt(pos).BlockType;
                    if (blockType == BlockTypes.FuelCell || blockType == BlockTypes.Reflector || blockType == BlockTypes.Irradiator)
                    {
                        primable = false;
                        break;
                    }
                    pos = Position + ++i * offset;
                }
                if(primable)
                    return true;
            }
            return false;
        }

        public void UnPrime()
        {
            Primed = UsedFuel.SelfPriming;
            NeutronSource = UsedFuel.SelfPriming ? "Self":  "None";
        }

        public void CyclePrimed()
        {
            if (UsedFuel.SelfPriming)
                return;
            if(Primed)
            {
                int currIndex = Palette.NeutronSourceNames.IndexOf(NeutronSource);
                if (currIndex != -1 && currIndex < Palette.NeutronSourceNames.Count - 1)
                    NeutronSource = Palette.NeutronSourceNames[currIndex + 1];
                else
                {
                    NeutronSource = "None";
                    Primed = false;
                    return;
                }
            }
            else
            {
                if(Palette.NeutronSourceNames.Count == 0)
                    throw new IndexOutOfRangeException("There were no Neutron Sources in the configuration!");
                NeutronSource = Palette.NeutronSourceNames[0];
                Primed = true;
            }
        }

        private double CalculateEfficiency()
        {
            double eff = PositionalEfficiency * UsedFuel.BaseEfficiency * (1 / (1 + Math.Exp(2 * (ModeratedNeutronFlux - 2 * UsedFuel.CriticalityFactor))));
            if(NeutronSource != "None" && NeutronSource != "Self")
                eff *= Configuration.NeutronSources[NeutronSource].Efficiency;
            return eff;
        }

        public void Activate()
        {
            Active = true;
        }

        public override Block Copy(Vector3 newPosition)
        {
            return new FuelCell(this, newPosition, this.UsedFuel, Primed);
        }

        public string ToSaveString()
        {
            return string.Join(";", UsedFuel.Name, Primed.ToString(), NeutronSource);
        }
    }
}
