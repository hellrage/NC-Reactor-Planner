using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;
using System.Text;

namespace NC_Reactor_Planner
{
    public class Reflector : Block
    {
        public override bool Valid { get => Active; }
        public bool Active { get; set; }
        public double ReflectivityMultiplier { get => Configuration.Reflectors[ReflectorType].ReflectivityMultiplier; }
        public double EfficiencyMultiplier { get => Configuration.Reflectors[ReflectorType].EfficiencyMultiplier; }
        public string ReflectorType { get; private set; }
        private List<FuelCell> adjacentFuelCells;

        public Reflector(string displayName, string type, Bitmap texture, Vector3 position) : base(displayName, BlockTypes.Reflector, texture, position)
        {
            ReflectorType = type;
            RevertToSetup();
        }

        public Reflector(Reflector parent, Vector3 position) : this(parent.DisplayName, parent.ReflectorType, parent.Texture, position)
        {
        }

        public override void RevertToSetup()
        {
            adjacentFuelCells = new List<FuelCell>();
            Active = false;
        }

        public void UpdateStats()
        {
            if (adjacentFuelCells.FindIndex(fuelCell => fuelCell.Valid) != -1)
                Active = true;
            else
                --Reactor.functionalBlocks;
        }

        public void AddAdjacentFuelCell(FuelCell fuelCell)
        {
            adjacentFuelCells.Add(fuelCell);
        }

        public override string GetToolTip()
        {
            StringBuilder result = new StringBuilder();
            result.Append(DisplayName);
            result.AppendLine(" reflector");
            if (Position == Palette.dummyPosition)
            {
                result.Append("This block reflects neutrons back\r\n" +
                    "to FuelCells through moderator lines.\r\n" +
                    "Increases FuelCell's flux by " + ReflectivityMultiplier * 200 + "%\r\n" +
                    "of the moderator line's sum flux.\r\n" +
                    "Increases heat multiplier.\r\n" +
                    "Only adds " + EfficiencyMultiplier * 100 + "% of positional efficiency.\r\n" +
                    "Must be no farther than " + Configuration.Fission.NeutronReach / 2 + " moderators\r\n" +
                    "away from a FuelCell.");
                return result.ToString();
            }
            result.AppendLine($"Reflectivity: {ReflectivityMultiplier}");
            result.AppendLine($"Efficiency multiplier: {EfficiencyMultiplier}");
            result.AppendLine(Valid?"":"--Not connected to an active FuelCell");
            return result.ToString();
        }

        public override Block Copy(Vector3 newPosition)
        {
            return new Reflector(this, newPosition);
        }
    }
}
