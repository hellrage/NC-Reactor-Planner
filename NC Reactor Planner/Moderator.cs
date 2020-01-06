using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public class Moderator : Block
    {
        public bool Active { get; set; }
        public string ModeratorType { get; private set; }
        public int FluxFactor { get => Configuration.Moderators[ModeratorType].FluxFactor; }
        public double EfficiencyFactor { get => Configuration.Moderators[ModeratorType].EfficiencyFactor; }
        public override bool Valid { get => Active & HasAdjacentValidFuelCell; }
        public bool HasAdjacentValidFuelCell { get; private set; }

        public Moderator(string displayName, string type, Bitmap texture, Vector3 position) : base(displayName, BlockTypes.Moderator, texture, position)
        {
            Active = false;
            HasAdjacentValidFuelCell = false;
            ModeratorType = type;
        }

        public Moderator(Moderator parent, Vector3 position) : this(parent.DisplayName, parent.ModeratorType, parent.Texture, position)
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
            for(int p = 1; p < 5; p *= 2)
            {
                Vector3 offset = Reactor.sixAdjOffsets[p];
                Tuple<int, BlockTypes> toOffset = WalkLineToValidSource(offset);
                Tuple<int, BlockTypes> oppositeOffset = WalkLineToValidSource(-offset);
                if (toOffset.Item1 > 0 & oppositeOffset.Item1 > 0)
                {
                    Active = true;
                    if (toOffset.Item1 == 1 & toOffset.Item2 == BlockTypes.FuelCell || oppositeOffset.Item1 == 1 & oppositeOffset.Item2 == BlockTypes.FuelCell)
                    {
                        HasAdjacentValidFuelCell = true;
                        return;
                    }
                }
            }
            if (!Active)
                --Reactor.functionalBlocks;
        }

        public Tuple<int, BlockTypes> WalkLineToValidSource(Vector3 offset)
        {
            int i = 0;
            while (++i <= Configuration.Fission.NeutronReach)
            {
                Vector3 pos = Position + i * offset;
                Block block = Reactor.BlockAt(pos);
                if (Reactor.interiorDims.X >= pos.X & Reactor.interiorDims.Y >= pos.Y & Reactor.interiorDims.Z >= pos.Z & pos.X > 0 & pos.Y > 0 & pos.Z > 0 & i <= Configuration.Fission.NeutronReach)
                {
                    if (block.BlockType == BlockTypes.FuelCell)
                        if (block.Valid)
                            return Tuple.Create(i, BlockTypes.FuelCell);
                    if(block.BlockType == BlockTypes.Reflector)
                        if(block.Valid & i < Configuration.Fission.NeutronReach / 2 + 1)
                            return Tuple.Create(i, BlockTypes.Reflector);
                    if (block.BlockType != BlockTypes.Moderator)
                        return Tuple.Create(-1, BlockTypes.Air);
                }
                else
                    return Tuple.Create(-1, BlockTypes.Air);
            }
            return Tuple.Create(-1, BlockTypes.Air);
        }

        public override string GetToolTip()
        {
            string toolTip = DisplayName + " moderator\r\n";
            if (Position != Palette.dummyPosition)
            {
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

        public override Block Copy(Vector3 newPosition)
        {
            return new Moderator(this, newPosition);
        }
    }
}
