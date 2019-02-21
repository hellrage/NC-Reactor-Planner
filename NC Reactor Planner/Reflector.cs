using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Drawing;

namespace NC_Reactor_Planner
{
    public class Reflector : Block
    {
        public override bool Valid { get => Active; }
        public bool Active { get; set; }
        private List<FuelCell> adjacentFuelCells;

        public Reflector(string displayName, Bitmap texture, Point3D position) : base(displayName, BlockTypes.Reflector, texture, position)
        {
            RevertToSetup();
        }

        public Reflector(Reflector parent, Point3D position) : this(parent.DisplayName, parent.Texture, position)
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
                    "heat multiplier but not efficiency.\t\n" +
                    "Must be no farther than " + Configuration.Fission.NeutronReach / 2 + " moderators\r\n" +
                    "away from a FuelCell.";
            return  tooltip +
                    (Valid?"":"--Has no neighbouring active FuelCells\r\n");
        }

        public override Block Copy(Point3D newPosition)
        {
            return new Reflector(this, newPosition);
        }
    }
}
