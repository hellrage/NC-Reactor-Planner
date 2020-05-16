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
        public double HeatMultiplier { get => AdjacentModeratorLines + (UsedFuel.SelfPriming ? 1 : 0); }
        public List<FuelCell> AdjacentCells { get; private set; }
        public List<Reflector> AdjacentReflectors { get; private set; }
        public List<Irradiator> AdjacentIrradiators { get; private set; }
        public int AdjacentModeratorLines { get; private set; }
        public bool Active { get; private set; }
        public override bool Valid { get => Active; }
        public bool Shielded { get; private set; }
        public Fuel UsedFuel { get; private set; }
        public double PositionalEfficiency { get; private set; }
        public int ModeratedNeutronFlux { get; private set; }
        public double Efficiency { get => CalculateEfficiency(); }
        public double FuelDuration { get => UsedFuel.FuelTime * Reactor.clusters[Cluster].FuelDurationMultiplier / Configuration.Fission.FuelUse; }
        public bool Primed { get; set; }
        public string NeutronSource { get; private set; }

        public FuelCell(string displayName, Bitmap texture, Vector3 position, Fuel usedFuel, bool primed = false, string neutronSource = "None") : base(displayName, BlockTypes.FuelCell, texture, position)
        {
            UsedFuel = usedFuel;
            Primed = primed || UsedFuel.SelfPriming;
            NeutronSource = neutronSource;
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
            List<NeutronShield> neutronShields = new List<NeutronShield>();
            Vector3 pos = Position + offset;
            int i = 1;
            while (Reactor.interiorDims.X >= pos.X & Reactor.interiorDims.Y >= pos.Y & Reactor.interiorDims.Z >= pos.Z & pos.X > 0 & pos.Y > 0 & pos.Z > 0 & i <= Configuration.Fission.NeutronReach + 1)
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
                        foreach (NeutronShield neutronShield in neutronShields)
                            neutronShield.ModeratedNeutronFlux += (int)(sumModeratorFlux/2);
                        return fuelCell;
                    }
                    else
                        return null;
                }
                else if (block is Irradiator irradiator && i > 1 && !this.AdjacentIrradiators.Contains(irradiator))
                {
                    irradiator.ModeratedNeutronFlux += sumModeratorFlux;
                    this.AdjacentIrradiators.Add(irradiator);
                    this.PositionalEfficiency += sumModeratorEfficiency * irradiator.EfficiencyMultiplier / moderatorsInLine;
                    foreach (NeutronShield neutronShield in neutronShields)
                        neutronShield.ModeratedNeutronFlux += sumModeratorFlux;
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
                    foreach (NeutronShield neutronShield in neutronShields)
                        neutronShield.ModeratedNeutronFlux += (int)(2 * sumModeratorFlux * reflector.ReflectivityMultiplier);
                    return null;
                }
                else if(block is NeutronShield neutronShield)
                {
                    if (neutronShield.Active)
                        return null;

                    neutronShields.Add(neutronShield);
                    sumModeratorEfficiency += neutronShield.EfficiencyFactor;
                    moderatorsInLine++;
                    i++;
                    pos = Position + offset * i;
                    continue;
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
                --Reactor.functionalBlocks;
                AdjacentCells.Clear();
                AdjacentModeratorLines = 0;
                return;
            }
            AdjacentCells = AdjacentCells.FindAll(fc => fc.Active);
            AdjacentModeratorLines = AdjacentCells.Count + AdjacentReflectors.Count + AdjacentIrradiators.Count;
        }

        public bool CanBePrimed()
        {
            bool primable;
            foreach (Vector3 offset in Reactor.sixAdjOffsets)
            {
                int i = 1;
                primable = true;
                Vector3 pos = Position + i * offset;
                while (Reactor.interiorDims.X + 1 >= pos.X & Reactor.interiorDims.Y + 1 >= pos.Y & Reactor.interiorDims.Z + 1 >= pos.Z & pos.X >= 0 & pos.Y >= 0 & pos.Z >= 0)
                {
                    BlockTypes blockType = Reactor.BlockAt(pos).BlockType;
                    if (blockType == BlockTypes.FuelCell | blockType == BlockTypes.Reflector | blockType == BlockTypes.Irradiator)
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
            Primed = false;
            NeutronSource = "None";
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
            if(NeutronSource != "None")
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
