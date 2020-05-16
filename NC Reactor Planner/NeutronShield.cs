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

        public void UpdateStats()
        {
            if (!Valid)
                Reactor.functionalBlocks--;
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
