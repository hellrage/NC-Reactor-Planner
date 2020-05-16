using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public class Irradiator: Block
    {
        public int ModeratedNeutronFlux { get; set; }
        public int HeatPerFlux { get; set; }
        public int HeatPerTick { get => ModeratedNeutronFlux * HeatPerFlux; }
        public double EfficiencyMultiplier { get; set; }
        public override bool Valid { get => ModeratedNeutronFlux > 0; }
        public override bool ReducesSparsity => Valid;

        public Irradiator(string displayName, Bitmap texture, Vector3 position, IrradiatorValues values) : base(displayName, BlockTypes.Irradiator, texture, position)
        {
            this.HeatPerFlux = values.HeatPerFlux;
            this.EfficiencyMultiplier = values.EfficiencyMultiplier;
            RevertToSetup();
        }

        public Irradiator(string displayName, Bitmap texture, Vector3 position) : base(displayName, BlockTypes.Irradiator, texture, position)
        {
            this.HeatPerFlux = Configuration.Fission.IrradiatorHeatPerFlux;
            this.EfficiencyMultiplier = Configuration.Fission.IrradiatorEfficiencyMultiplier;
            RevertToSetup();
        }

        public Irradiator(Irradiator parent, Vector3 newPosition) : this(parent.DisplayName, parent.Texture, newPosition)
        {
            this.HeatPerFlux = parent.HeatPerFlux;
            this.EfficiencyMultiplier = parent.EfficiencyMultiplier;
        }

        public override void RevertToSetup()
        {
            ModeratedNeutronFlux = 0;
            SetCluster(-1);
        }

        public void Update()
        {
            for (int p = 0; p < 6; p++)
            {
                Vector3 offset = Reactor.sixAdjOffsets[p];
                WalkLineToValidSource(offset);
            }
        }

        private void WalkLineToValidSource(Vector3 offset)
        {
            int i = 0;
            int sumModeratorFlux = 0;
            double sumModeratorEfficiency = 0;
            int moderatorsInLine = 0;
            while (++i <= Configuration.Fission.NeutronReach + 1)
            {
                Vector3 pos = Position + i * offset;
                if (Reactor.PositionInsideInterior(pos))
                {
                    Block block = Reactor.BlockAt(pos);
                    if (block is FuelCell fuelCell)
                    {
                        if (fuelCell.Active)
                        {
                            this.ModeratedNeutronFlux += sumModeratorFlux;
                            fuelCell.PositionalEfficiency += sumModeratorEfficiency * EfficiencyMultiplier / moderatorsInLine;
                        }
                        return;
                    }
                    if (block is NeutronShield neutronShield)
                    {
                        if (neutronShield.Active)
                            return;
                        else
                            continue;
                    }
                    if (block is Moderator moderator)
                    {
                        moderatorsInLine++;
                        sumModeratorFlux += moderator.FluxFactor;
                        sumModeratorEfficiency += moderator.EfficiencyFactor;
                        continue;
                    }

                    return;

                }
            }
        }

        public override string GetToolTip()
        {
            StringBuilder tb = new StringBuilder(); //TooltipBuilder
            tb.AppendLine("Irradiator");
            tb.Append(base.GetToolTip());

            if (Position == Palette.dummyPosition)
            {
                tb.AppendLine("Direct flux into this block");
                tb.AppendLine("to perform recipes.");
                tb.AppendLine("Increases heat multiplier.");
                tb.AppendLine($"Only adds {EfficiencyMultiplier*100}% of positional efficiency.");
                tb.AppendLine("Heat per flux: " + HeatPerFlux);
                tb.AppendLine("Change values in the configuration to set your recipe.");
                tb.AppendLine("Each placed irradiator is unique (like fuel cells),");
                tb.AppendLine("so you can have multiple different ones in the same reactor.");
            }
            else
            {
                tb.AppendLine("Total flux: " + ModeratedNeutronFlux);
                tb.AppendLine("Heat per flux: " + HeatPerFlux);
                tb.AppendLine("Total heat: " + HeatPerTick);
                tb.AppendLine($"Adds {EfficiencyMultiplier * 100}% of positional efficiency.");
            }

            return tb.ToString();
        }

        public override Block Copy(Vector3 newPosition)
        {
            return new Irradiator(this, newPosition);
        }
    }
}
