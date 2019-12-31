using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Media3D;
using System.Text.RegularExpressions;

namespace NC_Reactor_Planner
{
    public class HeatSink : Block
    {
        private List<string> placementErrors;
        
        public double Cooling { get; private set; }
        public string Requirements { get; private set; }
        public override bool Valid { get; protected set; }
        public List<Func<Point3D,List<string>,bool>> Validators { get; private set; }
        public List<string> Dependencies { get; private set; }

        public string HeatSinkType { get; private set; }


        public HeatSink(string displayName, Bitmap texture, string type, double heatPassive, string requirements, Point3D position) : base(displayName, BlockTypes.HeatSink, texture, position)
        {
            HeatSinkType = type;
            Cooling = heatPassive;
            Requirements = requirements;
            Valid = false;
            Validators = new List<Func<Point3D, List<string>, bool>>();
            Dependencies = new List<string>();
            placementErrors = new List<string>();
        }

        public HeatSink(HeatSink parent, Point3D position) : this(parent.DisplayName, parent.Texture, parent.HeatSinkType, parent.Cooling, parent.Requirements, position)
        {
            Validators = parent.Validators;
        }

        public void ConstructValidators()
        {
            Validators.Clear();

            string[] numberStrings = new string[] { "One", "Two", "Three", "Four", "Five", "Six" };
            Dictionary<string, byte> nums = new Dictionary<string, byte>();
            for (int i = 0; i < 6; i++)
                nums.Add(numberStrings[i], (byte)(i+1));
            //string numbersRegex = string.Join("|", numberStrings);
            //Regex ruleFormat = new Regex(@"^(Exact |Vertex |Axial )?("+numbersRegex+@")\s()");

            string[] rules = Requirements.Split(';');
            foreach (string rule in rules)
            {
                //string trimmedRule = rule.Trim();
                //Match ruleMatch = ruleFormat.Match(trimmedRule);
                //var words = trimmedRule.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var words = rule.Trim().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                switch (words[0])
                {
                    case "Exactly":
                        if (!rule.Contains("heatsink"))
                            if (nums[words[1]] > 1)
                                words[2] = words[2].Substring(0, words[2].Length - 1);
                        if(!Dependencies.Contains(words[2]))
                            Dependencies.Add(words[2]);

                        if (words[2] == "Moderator")
                            Validators.Add((pos, errs) => { return HasAdjacent(pos, errs, Palette.BlockPalette["Graphite"], nums[words[1]], true); });
                        else if (words[2] == "Casing")
                            Validators.Add((pos, errs) => { return HasAdjacent(pos, errs, Palette.dummyCasing, nums[words[1]], true); });
                        else
                            Validators.Add((pos, errs) => { return HasAdjacent(pos, errs, Palette.BlockPalette[words[2]], nums[words[1]], true); });
                        break;
                    case "Vertex":
                        List<Block> vertexBlocks = new List<Block>();
                        List<string> vbNames = new List<string>() { words[3].Replace(",", ""), words[words.IndexOf("One", 3) + 1].Replace(",", ""), words[words.IndexOf("One", 6) + 1] };
                        foreach (string name in vbNames)
                        {
                            if (!Dependencies.Contains(name))
                                Dependencies.Add(name);

                            if (name == "Moderator")
                                vertexBlocks.Add(Palette.BlockPalette["Graphite"]);
                            else if (name == "Casing")
                                vertexBlocks.Add(Palette.dummyCasing);
                            else
                                vertexBlocks.Add(Palette.BlockPalette[name]);
                        }
                        Validators.Add((pos, errs) => { return HasVertex(pos, errs, vertexBlocks); });
                        break;
                    case "Axial":
                        if (!rule.Contains("heatsink"))
                            words[1] = words[1].Substring(0, words[1].Length - 1);

                        if (!Dependencies.Contains(words[1]))
                            Dependencies.Add(words[1]);

                        if (words[1] == "Moderator")
                            Validators.Add((pos, errs) => { return HasAxial(pos, errs, Palette.BlockPalette["Graphite"]); });
                        else if (words[1] == "Casing")
                            Validators.Add((pos, errs) => { return HasAxial(pos, errs, Palette.dummyCasing); });
                        else
                            Validators.Add((pos, errs) => { return HasAxial(pos, errs, Palette.BlockPalette[words[1]]); });
                        break;
                    case "One":
                    case "Two":
                    case "Three":
                    case "Four":
                    case "Five":
                    case "Six":
                        if (!rule.Contains("heatsink"))
                            if (nums[words[0]] > 1)
                                words[1] = words[1].Substring(0, words[1].Length - 1);

                        if (!Dependencies.Contains(words[1]))
                            Dependencies.Add(words[1]);

                        if (words[1] == "Moderator")
                            Validators.Add((pos, errs) => { return HasAdjacent(pos, errs, Palette.BlockPalette["Graphite"], nums[words[0]], false); });
                        else if (words[1] == "Casing")
                            Validators.Add((pos, errs) => { return HasAdjacent(pos, errs, Palette.dummyCasing, nums[words[0]], false); });
                        else
                        Validators.Add((pos, errs) => { return HasAdjacent(pos, errs, Palette.BlockPalette[words[1]], nums[words[0]], false); });
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show(string.Format("Invalid rule string in {0}!\r\n{1}", HeatSinkType, rule));
                        Requirements = "Invalid requirements!";
                        Validators.Clear();
                        return;
                }
            }
        }

        public override string GetToolTip()
        {
            StringBuilder report = new StringBuilder();
            report.Append(DisplayName);
            report.Append(" heatsink\r\n");

            if (Position != Palette.dummyPosition)
            {
                if (Cluster != -1)
                {
                    //[TODO] Consolidate with Block tooltip
                    report.Append(string.Format("Cluster: {0}\r\n", Cluster));
                    report.Append((Reactor.clusters[Cluster].HasPathToCasing ? " Has casing connection\r\n" : "--Invalid cluster!\r\n--No casing connection"));
                    if (Reactor.clusters[Cluster].PenaltyType > 0)
                        report.Append("--Cluster is penalized for overheating!\n");
                    else if (Reactor.clusters[Cluster].PenaltyType < 0)
                        report.Append("--Cluster is penalized for overcooling!\n");
                }
                else
                    report.Append("--No cluster!\r\n");
            }

            report.Append(string.Format(" Cooling: {0} HU/t\r\n", Cooling));
            report.Append(string.Format(" Requires: {0}\r\n", Requirements));
            if (Position != Palette.dummyPosition & !Valid)
            {
                foreach (string error in new HashSet<string>(placementErrors))
                {
                    report.Append(string.Format("----{0}\r\n", error));
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

        public override void ReloadValuesFromConfig()
        {
            HeatSinkValues cv = Configuration.HeatSinks[HeatSinkType];
            Cooling = cv.HeatPassive;
            Requirements = cv.Requirements;
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
                --Reactor.functionalBlocks;
                Valid = false;
            }

            if (!Valid)
                --Reactor.functionalBlocks;
            return Valid;
        }

        private static bool HasAdjacent(Point3D Position, List<string> placementErrors, Block needed, int number = 1, bool exact = false)
        {
            int adjacent = 0;
            int activeAdjacent = 0;
            foreach (Vector3D o in Reactor.sixAdjOffsets)
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
                placementErrors.Add("Too many " + ((needed.BlockType == BlockTypes.Moderator) ? "Moderators" : needed.DisplayName + "s"));
                return false;
            }

            if (adjacent == 0)
            {
                placementErrors.Add("No " + ((needed.BlockType == BlockTypes.Moderator) ? "Moderator" : needed.DisplayName));
                return false;
            }

            if(activeAdjacent < number)
            {
                if (adjacent < number)
                    placementErrors.Add("Too few " + ((needed.BlockType == BlockTypes.Moderator) ? "Moderators" : needed.DisplayName + "s"));
                else
                    placementErrors.Add("Invalid " + ((needed.BlockType == BlockTypes.Moderator) ? "Moderator" : needed.DisplayName));
                return false;
            }

            return true;
        }

        private static bool HasAxial(Point3D Position, List<string> placementErrors, Block needed)
        {
            BlockTypes bn = needed.BlockType;
            byte status = 0; //0:none, 1: invalid, 2: valid

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
                                status = 2;
                                break;
                            }
                        }
                    }
                    else
                    {
                        status = Math.Max(status, (byte)1);
                        if (block1.Valid && block2.Valid)
                        {
                            status = 2;
                            break;
                        }
                    }
                }
            }

            switch (status)
            {
                case 0:
                    placementErrors.Add("No axial " + ((needed.BlockType == BlockTypes.Moderator) ? "Moderators" : needed.DisplayName + "s"));
                    return false;
                case 1:
                    placementErrors.Add("Invalid " + ((needed.BlockType == BlockTypes.Moderator) ? "Moderator" : needed.DisplayName));
                    return false;
                case 2:
                    return true;
                default:
                    placementErrors.Add("Unknown status!");
                    return false;
            }
        }

        private static bool HasVertex(Point3D Position, List<string> placementErrors, List<Block> needed)
        {
            if (needed.Count != 3)
                throw new ArgumentException("Vertex rules need exactly 3 blocks specified");

            List<List<Vector3D>> eligible = new List<List<Vector3D>>();
            for (int i = 0; i < 3; i++)
                eligible.Add(new List<Vector3D>());

            byte[] status = new byte[3] { 0, 0, 0 };//first, second, third needed block - 0:none, 1:inactive, 2:valid

            void ProcessStatus()
            {
                for (int i = 0; i < 3; i++)
                {
                    switch (status[i])
                    {
                        case 0:
                            placementErrors.Add("No " + ((needed[i].BlockType == BlockTypes.Moderator) ? "Moderator" : needed[i].DisplayName));
                            break;
                        case 1:
                            placementErrors.Add("Invalid " + ((needed[i].BlockType == BlockTypes.Moderator) ? "Moderator" : needed[i].DisplayName));
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

            foreach (Vector3D a in eligible[0])
            {
                foreach (Vector3D b in eligible[1])
                {
                    foreach (Vector3D c in eligible[2])
                    {
                        if (Vector3D.DotProduct(Vector3D.CrossProduct(a, b), c) != 0)
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

        public override Block Copy(Point3D newPosition)
        {
            return new HeatSink(this, newPosition);
        }
    }
}
