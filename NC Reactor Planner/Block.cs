using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public class Block
    {
        public string DisplayName { get; private set; }
        public Bitmap Texture { get; set; }
        public Vector3 Position { get; private set; }
        public BlockTypes BlockType { get; private set; }
        public int Cluster { get; private set; }
        public virtual bool Valid { get { return true; } protected set { } }

        public Block()
        {
            DisplayName = "Air";
            Texture = Palette.Textures["Air"];
            Position = Palette.dummyPosition;
        }

        public Block(string displayName, BlockTypes blockType, Bitmap texture, Vector3 position, int clusterID = -1)
        {
            DisplayName = displayName;
            Texture = texture;
            Position = position;
            BlockType = blockType;
            Cluster = clusterID;
        }

        public virtual void SetCluster(int clusterID)
        {
            Cluster = clusterID;
        }

        public virtual void RevertToSetup(){}

        public virtual string GetToolTip()
        {
            StringBuilder report = new StringBuilder();
            if (BlockType == BlockTypes.Air)
                report.AppendLine(DisplayName);
            if (Position != Palette.dummyPosition)
            {
                if (Cluster != -1)
                {
                    report.AppendLine($"Cluster: {Cluster}");
                    report.AppendLine((Reactor.clusters[Cluster].HasPathToCasing ? " Has casing connection" : $"--Invalid cluster!{Environment.NewLine}--No casing connection"));
                    if (Reactor.clusters[Cluster].NetHeatClass == NetHeatClass.Overheating)
                        report.AppendLine("--Cluster is penalized for overheating!");
                    else if (Reactor.clusters[Cluster].NetHeatClass == NetHeatClass.Overcooled)
                        report.AppendLine("--Cluster is penalized for overcooling!");
                    else if (Reactor.clusters[Cluster].NetHeatClass == NetHeatClass.HeatPositive)
                        report.AppendLine("--Cluster is heat positive!");
                }
                else if (BlockType != BlockTypes.Air && BlockType != BlockTypes.Reflector)
                    report.AppendLine("--No cluster!");
            }
            return report.ToString();
        }

        public virtual Block Copy(Vector3 newPosition)
        {
            return new Block(DisplayName, BlockType, Texture, newPosition);
        }

        public virtual Dictionary<string, int> GetResourceCosts()
        {
            return new Dictionary<string, int>();
        }
    }
}
