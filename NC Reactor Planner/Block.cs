using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public class Block
    {
        private bool _valid;
        public string DisplayName { get; private set; }
        public Bitmap Texture { get; set; }
        public Vector3 Position { get; private set; }
        public BlockTypes BlockType { get; private set; }
        public int Cluster { get; private set; }
        public virtual bool Valid { get => true; protected set => _valid = value; }

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

        public virtual void RevertToSetup()
        {
        }

        public virtual string GetToolTip()
        {
            StringBuilder report = new StringBuilder();
            report.Append(DisplayName + "\r\n");
            if (Position != Palette.dummyPosition)
            {
                if (Cluster != -1)
                {
                    //[TODO] Consolidate with HeatSink tooltip
                    report.Append(string.Format("Cluster: {0}\r\n", Cluster));
                    if (Reactor.clusters[Cluster].PenaltyType > 0)
                        report.Append("--Cluster is penalized for overheating!\n");
                    else if (Reactor.clusters[Cluster].PenaltyType < 0)
                        report.Append("--Cluster is penalized for overcooling!\n");
                }
                else if (BlockType != BlockTypes.Air && BlockType != BlockTypes.Reflector)
                    report.Append("--No cluster!\r\n");
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
