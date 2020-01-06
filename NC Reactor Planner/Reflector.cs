using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;

namespace NC_Reactor_Planner
{
    public class Reflector : Block
    {
        public override bool Valid { get => Active; }
        public bool Active { get; set; }
        private List<FuelCell> adjacentFuelCells;

        public Reflector(string displayName, Bitmap texture, Vector3 position) : base(displayName, BlockTypes.Reflector, texture, position)
        {
            RevertToSetup();
        }

        public Reflector(Reflector parent, Vector3 position) : this(parent.DisplayName, parent.Texture, position)
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
            string tooltip = base.GetToolTip();
            if (Position == Palette.dummyPosition)
                return tooltip +
                    "This block reflects neutrons back\r\n" +
                    "to FuelCells through moderator lines.\r\n" +
                    "This increases the FuelCell's flux and\r\n" +
                    "heat multiplier but only adds "+Configuration.Fission.ReflectorEfficiency+"\r\n" +
                    "the positional efficiency.\r\n" +
                    "Must be no farther than " + Configuration.Fission.NeutronReach / 2 + " moderators\r\n" +
                    "away from a FuelCell.";
            return  tooltip +
                    (Valid?"":"--Not connected to an active FuelCell\r\n");
        }

        public override Block Copy(Vector3 newPosition)
        {
            return new Reflector(this, newPosition);
        }
    }
}
