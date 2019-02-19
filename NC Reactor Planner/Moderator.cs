using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class Moderator : Block
    {
        public bool Active { get; set; }
        public ModeratorTypes ModeratorType { get; private set; }
        public double FluxFactor { get; private set; }
        public double EfficiencyFactor { get; private set; }
        public override bool Valid { get => Active & HasAdjacentValidFuelCell; }
        public bool HasAdjacentValidFuelCell { get; private set; }

        public Moderator(string displayName, ModeratorTypes type, Bitmap texture, Point3D position, double fluxFactor, double efficiencyFactor) : base(displayName, BlockTypes.Moderator, texture, position)
        {
            FluxFactor = fluxFactor;
            EfficiencyFactor = efficiencyFactor;
            Active = false;
            HasAdjacentValidFuelCell = false;
            ModeratorType = type;
        }

        public Moderator(Moderator parent, Point3D position) : this(parent.DisplayName, parent.ModeratorType, parent.Texture, position, parent.FluxFactor, parent.EfficiencyFactor)
        {
            ModeratorType = parent.ModeratorType;
        }

        public override void RevertToSetup()
        {
            Active = false;
            HasAdjacentValidFuelCell = false;
        }

        public void UpdateStats()
        {
            foreach (Vector3D offset in Reactor.sixAdjOffsets)
            {
                int toOffset = WalkLineToValidFuelCell(offset);
                int oppositeOffset = WalkLineToValidFuelCell(-offset);
                if (toOffset > 0 & oppositeOffset > 0)
                {
                    Active = true;
                    if (toOffset == 1 || oppositeOffset == 1)
                    {
                        HasAdjacentValidFuelCell = true;
                        return;
                    }
                }
            }
        }

        public int WalkLineToValidFuelCell(Vector3D offset)
        {
            int i = 0;
            while (++i <= Configuration.Fission.NeutronReach)
            {
                Point3D pos = Position + i * offset;
                if (Reactor.interiorDims.X >= pos.X & Reactor.interiorDims.Y >= pos.Y & Reactor.interiorDims.Z >= pos.Z & pos.X > 0 & pos.Y > 0 & pos.Z > 0 & i <= Configuration.Fission.NeutronReach)
                {
                    if (Reactor.BlockAt(pos) is FuelCell fuelCell)
                        if (fuelCell.Valid)
                            return i;
                    if (!(Reactor.BlockAt(pos) is Moderator))
                        return -1;
                }
                else
                    return -1;
            }
            return -1;
        }

        public override string GetToolTip()
        {
            string toolTip = DisplayName + " Moderator\r\n";
            if (Position != Palette.dummyPosition)
            {
                toolTip += string.Format("at: X: {0} Y: {1} Z: {2}\r\n", Position.X, Position.Y, Position.Z);
                if(!Active)
                    toolTip += "--Inactive!\r\n";
                if(Active)
                    toolTip += "In an active moderator line\r\n";
                if(!HasAdjacentValidFuelCell)
                    toolTip += "Cannot support any heatsinks\r\n";
            }
            toolTip += string.Format("Flux Factor: {0}\r\n", FluxFactor);
            toolTip += string.Format("Efficiency Factor: {0}\r\n", EfficiencyFactor);
            return toolTip;
        }

        public override Block Copy(Point3D newPosition)
        {
            return new Moderator(this, newPosition);
        }

        public override void ReloadValuesFromConfig()
        {
            FluxFactor = Configuration.Moderators[DisplayName].FluxFactor;
            EfficiencyFactor = Configuration.Moderators[DisplayName].EfficiencyFactor;
        }

    }

    public enum ModeratorTypes
    {
        Beryllium,
        Graphite,
        HeavyWater,
        //NotAModerator,
    }
}
