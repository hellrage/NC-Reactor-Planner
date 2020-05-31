using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public class NeutronShield: Block
    {
        public bool Active { get; private set; }
        public string NeutronShieldType { get; private set; }
        public int ModeratedNeutronFlux { get; set; }
        public int HeatPerFlux { get => Configuration.NeutronShields[NeutronShieldType].HeatPerFlux; }
        public int HeatPerTick { get => ModeratedNeutronFlux * HeatPerFlux; }
        public double EfficiencyFactor { get => Configuration.NeutronShields[NeutronShieldType].EfficiencyFactor; }
        public override bool Valid { get => ModeratedNeutronFlux > 0 && !Active; }
        public override bool ReducesSparsity => Valid;

        public NeutronShield(string displayName, string neutronShieldType, Bitmap texture, Vector3 position) : base(displayName, BlockTypes.NeutronShield, texture, position)
        {
            this.NeutronShieldType = neutronShieldType;
            RevertToSetup();
        }

        public NeutronShield(NeutronShield parent, Vector3 newPosition) : this(parent.DisplayName, parent.NeutronShieldType, parent.Texture, newPosition)
        {
        }

        public override void RevertToSetup()
        {
            ModeratedNeutronFlux = 0;
            SetCluster(-1);
        }

        public void Activate()
        {
            this.Active = true;
            this.Texture = Palette.Textures[NeutronShieldType.Replace('-', '_') + "_On"];
        }

        public void Deactivate()
        {
            this.Active = false;
            this.Texture = Palette.Textures[NeutronShieldType.Replace('-', '_') + "_Off"];
        }

        public void Update()
        {
            if (Active)
                return;
            
            for (int p = 1; p < 5; p *= 2)
            {
                Vector3 offset = Reactor.sixAdjOffsets[p];
                Tuple<int, BlockTypes, int> toOffset = WalkLineToValidSource(offset);
                Tuple<int, BlockTypes, int> oppositeOffset = WalkLineToValidSource(-offset);
                if (toOffset.Item1 > 0 && oppositeOffset.Item1 > 0
                    && (toOffset.Item2 == BlockTypes.FuelCell || oppositeOffset.Item2 == BlockTypes.FuelCell))
                {
                    if (toOffset.Item2 == BlockTypes.Reflector || oppositeOffset.Item2 == BlockTypes.Reflector)
                    {
                        if (toOffset.Item1 + oppositeOffset.Item1 + 1 - 2 <= Configuration.Fission.NeutronReach / 2)
                        {
                            if (Reactor.BlockAt(Position + toOffset.Item1 * offset) is Reflector reflector)
                                ModeratedNeutronFlux += (int)((oppositeOffset.Item3 + toOffset.Item3) * (1 + reflector.ReflectivityMultiplier));
                            else if(Reactor.BlockAt(Position + oppositeOffset.Item1 * offset) is Reflector oppReflector)
                                ModeratedNeutronFlux += (int)((oppositeOffset.Item3 + toOffset.Item3) * (1 + oppReflector.ReflectivityMultiplier));
                        }
                    }
                    else if (toOffset.Item1 + oppositeOffset.Item1 + 1 - 2 <= Configuration.Fission.NeutronReach)
                    {
                        ModeratedNeutronFlux += toOffset.Item3 + oppositeOffset.Item3;
                    }
                }
            }
        }

        private Tuple<int, BlockTypes, int> WalkLineToValidSource(Vector3 offset)
        {
            int i = 0;
            int sumModeratorFlux = 0;
            while (++i <= Configuration.Fission.NeutronReach)
            {
                Vector3 pos = Position + i * offset;
                Block block = Reactor.BlockAt(pos);
                if (Reactor.PositionInsideInterior(pos))
                {
                    if (block.BlockType == BlockTypes.FuelCell && block.Valid)
                        return Tuple.Create(i, BlockTypes.FuelCell, sumModeratorFlux);
                    if (block.BlockType == BlockTypes.Irradiator && block.Valid)
                        return Tuple.Create(i, BlockTypes.Irradiator, 0);
                    if (block.BlockType == BlockTypes.Reflector && block.Valid)
                        return Tuple.Create(i, BlockTypes.Reflector, sumModeratorFlux);
                    if (block is NeutronShield neutronShield)
                    {
                        if (neutronShield.Active)
                            return Tuple.Create(-1, BlockTypes.NeutronShield, 0);
                        else
                            continue;
                    }
                    if (block is Moderator moderator)
                    {
                        sumModeratorFlux += moderator.FluxFactor;
                        continue;
                    }

                    return Tuple.Create(-1, BlockTypes.Air, 0);
                }
                else
                    return Tuple.Create(-1, BlockTypes.Air, 0);
            }
            return Tuple.Create(-1, BlockTypes.Air, 0);
        }

        public override string GetToolTip()
        {
            StringBuilder tb = new StringBuilder(); //TooltipBuilder
            tb.AppendLine(DisplayName + " neutron shield");
            tb.Append(base.GetToolTip());
            if (Active)
            {
                tb.AppendLine("Active! Blocking flux from passing through.");
                tb.AppendLine("Cannot form clusters.");
            }
            else
            {
                tb.AppendLine("Inactive! Lets flux through.");
                tb.AppendLine("Can form clusters.");
            }

            if (Position == Palette.dummyPosition)
            {
                tb.AppendLine("This block acts as a toggleable moderator.");
                tb.AppendLine("Shift-leftclick to toggle.");
                tb.AppendLine("Contributes no flux. Generates heat when inactive.");
                tb.AppendLine("Heat per flux: " + HeatPerFlux);
                tb.AppendLine($"Efficiency factor: {EfficiencyFactor}");
            }
            else
            {
                tb.AppendLine("Total flux: " + ModeratedNeutronFlux);
                tb.AppendLine("Heat per flux: " + HeatPerFlux);
                tb.AppendLine("Total heat: " + HeatPerTick);
                tb.AppendLine($"Efficiency factor: {EfficiencyFactor}");
            }

            return tb.ToString();
        }

        public override Block Copy(Vector3 newPosition)
        {
            return new NeutronShield(this, newPosition);
        }
    }
}
