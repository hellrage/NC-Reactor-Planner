using System;
using System.Collections.Generic;
using System.Drawing;

namespace NC_Reactor_Planner
{
    public class FuelCell : Block
    {
        public double EnergyProducedPerTick { get; private set; }
        public double HeatProducedPerTick { get; private set; }
        public double EnergyMultiplier { get; private set; }
        public double HeatMultiplier { get; private set; }
        public int AdjacentCells { get; private set; }
        public int AdjacentModerators { get; private set; }
        public override bool Valid { get => true; }


        public FuelCell(string displayName, Bitmap texture, Point3D position) : base(displayName, BlockTypes.FuelCell, texture, position)
        {
            AdjacentCells = 0;
            EnergyProducedPerTick = 0;
            HeatProducedPerTick = 0;
        }

        public FuelCell(FuelCell parent, Point3D position) : this(parent.DisplayName, parent.Texture, position)
        {
        }

        public override string GetToolTip()
        {
            if (Position == Palette.dummyPosition)
                return base.GetToolTip();
            else
                return string.Format("{0}" +
                                    " Adjacent cells: {1}\r\n" +
                                    " Adjacent moderators: {2}\r\n" +
                                    " Energy multiplier: {3} %\r\n" +
                                    " Energy produced: {4} RF/t\r\n" +
                                    " Heat multiplier: {5} %\r\n" +
                                    " Heat produced: {6} HU/t", base.GetToolTip(), AdjacentCells, AdjacentModerators, (int)(EnergyMultiplier*100), EnergyProducedPerTick, (int)(HeatMultiplier*100), HeatProducedPerTick);
        }

        public void UpdateStats()
        {
            AdjacentCells = FindAdjacentCells();
            AdjacentCells += FindModeratorsThenAdjacentCells();
            AdjacentModerators = FindAdjacentModerators();

            EnergyMultiplier = 0;
            HeatMultiplier = 0;
            EnergyProducedPerTick = 0;
            HeatProducedPerTick = 0;

            EnergyMultiplier += AdjacentCells + 1;
            HeatMultiplier += (AdjacentCells + 1) * (AdjacentCells + 2) / 2;

            Reactor.energyMultiplier += AdjacentCells + 1;
            Reactor.heatMultiplier += (AdjacentCells + 1) * (AdjacentCells + 2) / 2;

            EnergyProducedPerTick = Reactor.usedFuel.BasePower * Configuration.Fission.Power * (AdjacentCells + 1);
            HeatProducedPerTick = Reactor.usedFuel.BaseHeat * Configuration.Fission.HeatGeneration * (AdjacentCells + 1) * (AdjacentCells + 2) / 2;

            Reactor.totalEnergyPerTick += Reactor.usedFuel.BasePower * Configuration.Fission.Power * (AdjacentCells + 1);
            Reactor.totalHeatPerTick += Reactor.usedFuel.BaseHeat * Configuration.Fission.HeatGeneration * (AdjacentCells + 1) * (AdjacentCells + 2) / 2;

            EnergyMultiplier += Configuration.Fission.ModeratorExtraPower/6 * AdjacentModerators * (AdjacentCells + 1);
            HeatMultiplier += Configuration.Fission.ModeratorExtraHeat/6 * AdjacentModerators * (AdjacentCells + 1);

            Reactor.energyMultiplier += Configuration.Fission.ModeratorExtraPower/6 * AdjacentModerators * (AdjacentCells + 1);
            Reactor.heatMultiplier += Configuration.Fission.ModeratorExtraHeat/6 * AdjacentModerators * (AdjacentCells + 1);

            EnergyProducedPerTick += Reactor.usedFuel.BasePower * Configuration.Fission.Power * Configuration.Fission.ModeratorExtraPower/6 * AdjacentModerators * (AdjacentCells + 1);
            HeatProducedPerTick += Reactor.usedFuel.BaseHeat * Configuration.Fission.HeatGeneration * Configuration.Fission.ModeratorExtraHeat/6 * AdjacentModerators * (AdjacentCells + 1);

            Reactor.totalEnergyPerTick += Reactor.usedFuel.BasePower * Configuration.Fission.Power * Configuration.Fission.ModeratorExtraPower/6 * AdjacentModerators * (AdjacentCells + 1);
            Reactor.totalHeatPerTick += Reactor.usedFuel.BaseHeat * Configuration.Fission.HeatGeneration * Configuration.Fission.ModeratorExtraHeat/6 * AdjacentModerators * (AdjacentCells + 1);
        }

        public int FindModeratorsThenAdjacentCells()
        {
            int moderatorAdjacentCells = 0;
            foreach (Vector3D offset in Reactor.sixAdjOffsets)
            {
                moderatorAdjacentCells += FindModeratorThenAdjacentCell(offset);
            }

            return moderatorAdjacentCells;
        }

        public int FindModeratorThenAdjacentCell(Vector3D offset)
        {
            Point3D pos;

            for (int i = 1; i <= Configuration.Fission.NeutronReach; i++)
            {
                pos = Position + offset * i;
                if ((Reactor.interiorDims.X >= pos.X & Reactor.interiorDims.Y >= pos.Y & Reactor.interiorDims.Z >= pos.Z)
                    & (pos.X > 0 & pos.Y > 0 & pos.Z > 0))
                {
                    if (!(Reactor.BlockAt(pos) is Moderator))
                        return 0;
                }
                else
                    return 0;
                pos = Position + offset * (i + 1);
                if ((Reactor.interiorDims.X >= pos.X & Reactor.interiorDims.Y >= pos.Y & Reactor.interiorDims.Z >= pos.Z)
                    & (pos.X > 0 & pos.Y > 0 & pos.Z > 0))
                {
                    if ((Reactor.BlockAt(pos) is FuelCell))
                    {
                        for (int r = i; r > 0; r--)
                        {
                            pos = Position + offset * r;
                            ((Moderator)Reactor.BlockAt(pos)).Validate();
                        }
                        return 1;
                    }
                }
                else
                    return 0;
            }
            return 0;
        }

        public int FindAdjacentCells()
        {
            int adjCells = 0;
            foreach (Vector3D o in Reactor.sixAdjOffsets)
            {
                if (Reactor.BlockAt(Position + o) is FuelCell)
                    adjCells++;
            }
            return adjCells;
        }

        public int FindAdjacentModerators()
        {
            int adjModerators = 0;
            foreach (Vector3D o in Reactor.sixAdjOffsets)
            {
                if (Reactor.BlockAt(Position + o) is Moderator moderator)
                {
                    adjModerators++;
                    moderator.Validate();
                    moderator.Active = true;
                }
            }
            return adjModerators;
        }

        public override Block Copy(Point3D newPosition)
        {
            return new FuelCell(this, newPosition);
        }
    }
}
