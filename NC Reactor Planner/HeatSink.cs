using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public class HeatSink : Block
    {
        private List<string> placementErrors;
        
        public double Cooling { get => Configuration.HeatSinks[HeatSinkType].HeatPassive; }
        public string Requirements { get => Configuration.HeatSinks[HeatSinkType].Requirements; }
        public override bool Valid { get; protected set; }
        public List<Func<Vector3,List<string>,bool>> Validators { get => Palette.HeatSinkValidators[HeatSinkType]; }
        public List<string> Dependencies { get; private set; }

        public string HeatSinkType { get; private set; }


        public HeatSink(string displayName, Bitmap texture, string type, Vector3 position, List<string> dependencies = null) : base(displayName, BlockTypes.HeatSink, texture, position)
        {
            HeatSinkType = type;
            Valid = false;
            Dependencies = dependencies ?? new List<string>();
            placementErrors = new List<string>();
        }

        public HeatSink(HeatSink parent, Vector3 position) : this(parent.DisplayName, parent.Texture, parent.HeatSinkType, position)
        {
            Dependencies = parent.Dependencies;
        }

        public override string GetToolTip()
        {
            StringBuilder report = new StringBuilder();
            report.Append(DisplayName);
            report.AppendLine(" heatsink");

            if (Position != Palette.dummyPosition)
            {
                if (Cluster != -1)
                {
                    //TODO: Consolidate with Block tooltip
                    report.AppendLine($"Cluster: {Cluster}");
                    report.AppendLine((Reactor.clusters[Cluster].HasPathToCasing ? " Has casing connection" : $"--Invalid cluster!{Environment.NewLine}--No casing connection"));

                    if (Reactor.clusters[Cluster].NetHeatClass == NetHeatClass.Overheating)
                        report.AppendLine("--Cluster is penalized for overheating!");
                    else if (Reactor.clusters[Cluster].NetHeatClass == NetHeatClass.Overcooled)
                        report.AppendLine("--Cluster is penalized for overcooling!");
                    else if (Reactor.clusters[Cluster].NetHeatClass == NetHeatClass.HeatPositive)
                        report.AppendLine("--Cluster is heat positive!");
                }
                else
                    report.AppendLine("--No cluster!");
            }

            report.AppendLine($" Cooling: {Cooling} HU/t");
            report.AppendLine($" Requires: {Requirements}");
            if (Position != Palette.dummyPosition & !Valid)
            {
                report.AppendLine("----Invalid!");
                foreach (string error in new HashSet<string>(placementErrors))
                {
                    report.AppendLine($"----{error}");
                }
            }
            return report.ToString();
        }

        public void UpdateStats()
        {
            Validate();
        }

        public override void RevertToSetup()
        {
            SetCluster(-1);
        }

        public bool Validate()
        {
            placementErrors = new List<string>();

            if (Validators.Count != 0)
            {
                Valid = true;
                foreach (var validator in Validators)
                    Valid &= validator(Position, placementErrors);
            }
            else
            {
                Valid = false;
            }

            if (!Valid)
                --Reactor.functionalBlocks;
            return Valid;
        }

        public static bool HasAdjacent(Vector3 Position, List<string> placementErrors, Block needed, int number = 1, bool exact = false)
        {
            int adjacent = 0;
            int activeAdjacent = 0;
            foreach (Vector3 o in Reactor.sixAdjOffsets)
            {
                Block block = Reactor.BlockAt(Position + o);
                BlockTypes bt = block.BlockType;
                BlockTypes nt = needed.BlockType;

                //If checked block doesn't match at all
                //Either heatsink types are mismatched or the blocktype is mismatched
                if (((bt == BlockTypes.HeatSink & nt == BlockTypes.HeatSink) && ((HeatSink)block).HeatSinkType != ((HeatSink)needed).HeatSinkType) || bt != nt)
                    continue;

                adjacent++;

                if (block.Valid)
                {
                    if (exact)
                    {
                        if (++activeAdjacent > number)
                            break;
                    }
                    else
                    {
                        if (++activeAdjacent >= number)
                            break;
                    }
                }
            }

            if ((activeAdjacent > number) && exact)
            {
                placementErrors.Add("Too many " + ((needed.BlockType == BlockTypes.HeatSink) ? needed.DisplayName + "s" : needed.BlockType.ToString() + "s"));
                return false;
            }

            if (adjacent == 0)
            {
                placementErrors.Add("No " + ((needed.BlockType == BlockTypes.HeatSink) ? needed.DisplayName : needed.BlockType.ToString()));
                return false;
            }

            if(activeAdjacent < number)
            {
                if (adjacent < number)
                    placementErrors.Add("Too few " + ((needed.BlockType == BlockTypes.HeatSink) ? needed.DisplayName + "s" : needed.BlockType.ToString() + "s"));
                else
                    placementErrors.Add("Invalid " + ((needed.BlockType == BlockTypes.HeatSink) ? needed.DisplayName : needed.BlockType.ToString()));
                return false;
            }

            return true;
        }

        public static bool HasAxial(Vector3 Position, List<string> placementErrors, Block needed, int number = 2, bool exact = false)
        {
            BlockTypes bn = needed.BlockType;
            byte status = 0; //0:none, 1: invalid, 2: valid, 3: under exact amount, 4: over exact amount
            int found = 0;

            for (int i = 0; i < 3; i++)
            {
                Block block1 = Reactor.BlockAt(Position + Reactor.sixAdjOffsets[2 * i]);
                BlockTypes bt1 = block1.BlockType;
                Block block2 = Reactor.BlockAt(Position + Reactor.sixAdjOffsets[2 * i + 1]);
                BlockTypes bt2 = block2.BlockType;

                if (bt1 == bn && bt2 == bn)
                {
                    if (needed is HeatSink hs)
                    {
                        if (((HeatSink)block1).HeatSinkType == hs.HeatSinkType && (((HeatSink)block1).HeatSinkType == ((HeatSink)block2).HeatSinkType))
                        {
                            status = Math.Max(status, (byte)1);
                            if (block1.Valid && block2.Valid)
                            {
                                found += 2;
                                if(!exact && found >= number)
                                {
                                    status = 2;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        status = Math.Max(status, (byte)1);
                        if (block1.Valid && block2.Valid)
                        {
                            found += 2;
                            if (!exact && found >= number)
                            {
                                status = 2;
                                break;
                            }
                        }
                    }
                }
            }

            if(exact)
            {
                if (found > number)
                    status = 4;
                else if (found < number)
                    status = 3;
                else
                    status = 2;
            }

            switch (status)
            {
                case 0:
                    placementErrors.Add("No axial " + ((needed.BlockType == BlockTypes.HeatSink) ? needed.DisplayName + "s" : needed.BlockType.ToString() + "s"));
                    return false;
                case 1:
                    placementErrors.Add("Invalid " + ((needed.BlockType == BlockTypes.HeatSink) ? needed.DisplayName : needed.BlockType.ToString()));
                    return false;
                case 2:
                    return true;
                case 3:
                    placementErrors.Add("Not enough " + ((needed.BlockType == BlockTypes.HeatSink) ? needed.DisplayName + "s" : needed.BlockType.ToString() + "s"));
                    return false;
                case 4:
                    placementErrors.Add("Too many " + ((needed.BlockType == BlockTypes.HeatSink) ? needed.DisplayName + "s" : needed.BlockType.ToString() + "s"));
                    return false;
                default:
                    placementErrors.Add("Unknown status!");
                    return false;
            }
        }

        public static bool HasVertex(Vector3 Position, List<string> placementErrors, List<Block> needed)
        {
            if (needed.Count != 3)
                throw new ArgumentException("Vertex rules need exactly 3 blocks specified");

            List<List<Vector3>> eligible = new List<List<Vector3>>();
            for (int i = 0; i < 3; i++)
                eligible.Add(new List<Vector3>());

            byte[] status = new byte[3] { 0, 0, 0 };//first, second, third needed block - 0:none, 1:inactive, 2:valid

            void ProcessStatus()
            {
                for (int i = 0; i < 3; i++)
                {
                    switch (status[i])
                    {
                        case 0:
                            placementErrors.Add("No " + ((needed[i].BlockType == BlockTypes.HeatSink) ? needed[i].DisplayName : needed[i].BlockType.ToString()));
                            break;
                        case 1:
                            placementErrors.Add("Invalid " + ((needed[i].BlockType == BlockTypes.HeatSink) ? needed[i].DisplayName : needed[i].BlockType.ToString()));
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (var offset in Reactor.sixAdjOffsets)
            {
                Block nbr = Reactor.BlockAt(Position + offset);
                for (int i = 0; i < 3; i++)
                {
                    if (needed[i].BlockType == nbr.BlockType)
                    {
                        if (nbr is HeatSink nbrhs)
                        {
                            if (((HeatSink)needed[i]).HeatSinkType == nbrhs.HeatSinkType)
                            {
                                status[i] = Math.Max(status[i], (byte)1);
                                if (nbrhs.Valid)
                                {
                                    status[i] = 2;
                                    eligible[i].Add(offset);
                                }
                            }
                        }
                        else
                        {
                            status[i] = Math.Max(status[i], (byte)1); ;
                            if (nbr.Valid)
                            {
                                status[i] = 2;
                                eligible[i].Add(offset);
                            }
                        }
                    }
                }
            }

            if (eligible[0].Count == 0 || eligible[1].Count == 0 || eligible[2].Count == 0)
            {
                ProcessStatus();
                return false;
            }

            foreach (Vector3 a in eligible[0])
            {
                foreach (Vector3 b in eligible[1])
                {
                    foreach (Vector3 c in eligible[2])
                    {
                        if (Vector3.Dot(Vector3.Cross(a, b), c) != 0)
                        {
                            ProcessStatus();
                            return true;
                        }
                    }
                }
            }

            placementErrors.Add("Required blocks aren't on a single vertex!");
            return false;
        }

        public override Block Copy(Vector3 newPosition)
        {
            return new HeatSink(this, newPosition);
        }
    }
}
