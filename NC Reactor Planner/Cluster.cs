using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public class Cluster
    {
        public bool HasPathToCasing { get; set; }
        public double TotalHeatPerTick { get; private set; }
        public double TotalCoolingPerTick { get; private set; }
        public double Efficiency { get; private set; }
        public double FuelDurationMultiplier { get; private set; }
        public double CoolingPenaltyMultiplier { get; private set; }
        public double TotalOutput { get; private set; }
        public double NetHeatingRate { get => (TotalHeatPerTick - TotalCoolingPerTick); }
        public int PenaltyType { get; private set; }
        public double HeatMultiplier { get; private set; }
        public bool Valid { get => HasPathToCasing; }
        public int ID { get; private set; }

        public List<Block> blocks;

        public Cluster(int id)
        {
            blocks = new List<Block>();
            ID = id;
            HasPathToCasing = false;
            ResetValues();
        }

        private void ResetValues()
        {
            TotalCoolingPerTick = 0;
            TotalHeatPerTick = 0;
            FuelDurationMultiplier = 1;
            CoolingPenaltyMultiplier = 1;
            HeatMultiplier = 0;
            TotalOutput = 0;
        }

        public void AddBlock(Block block)
        {
            blocks.Add(block);
        }

        public void UpdateStats()
        {
            double sumEffOfFuelCells = 0;
            double sumHeatMulti = 0;
            ResetValues();
            List<FuelCell> fuelCells = new List<FuelCell>();
            foreach(Block block in blocks)
            {
                switch (block.BlockType)
                {
                    case BlockTypes.HeatSink:
                        TotalCoolingPerTick += ((HeatSink)block).Cooling;
                        break;
                    case BlockTypes.FuelCell:
                        FuelCell fuelCell = block as FuelCell;
                        fuelCells.Add(fuelCell);
                        TotalHeatPerTick += fuelCell.HeatProducedPerTick;
                        TotalOutput += fuelCell.Efficiency * fuelCell.UsedFuel.BaseHeat;
                        sumEffOfFuelCells += fuelCell.Efficiency;
                        sumHeatMulti += fuelCell.HeatMultiplier;
                        break;
                    case BlockTypes.Moderator:
                    case BlockTypes.Air:
                    case BlockTypes.Casing:
                    case BlockTypes.Conductor:
                    default:
                        throw new ArgumentException("Unexpected blockType in cluster: " + block.BlockType.ToString());
                }
            }

            double rawEfficiency = sumEffOfFuelCells / fuelCells.Count;
            if(TotalCoolingPerTick > 0 & TotalHeatPerTick > 0)
                CoolingPenaltyMultiplier = Math.Min(1, (TotalHeatPerTick + Configuration.Fission.CoolingPenaltyLeniency) / TotalCoolingPerTick);

            Efficiency = rawEfficiency * CoolingPenaltyMultiplier;
            TotalOutput *= CoolingPenaltyMultiplier;

            if (TotalCoolingPerTick > 0 & TotalHeatPerTick > 0)
                FuelDurationMultiplier = Math.Min(1, (TotalCoolingPerTick + Configuration.Fission.CoolingPenaltyLeniency) / TotalHeatPerTick);
            else
                FuelDurationMultiplier = 1;
            HeatMultiplier = sumHeatMulti / fuelCells.Count;

            if (NetHeatingRate < -Configuration.Fission.CoolingPenaltyLeniency)
                PenaltyType = -1;
            else if (NetHeatingRate > Configuration.Fission.CoolingPenaltyLeniency)
                PenaltyType = 1;
            else
                PenaltyType = 0;
        }

        public string GetStatString()
        {
            if (!Valid)
                return string.Format("Cluster №{0}\r\nInvalid! Skipping.\r\n\r\n", ID);
            StringBuilder stats = new StringBuilder();
            stats.Append(string.Format("Cluster №{0}\r\n", ID));
            stats.Append(string.Format("Total output: {0}\r\n", TotalOutput));
            stats.Append(string.Format("Efficiency: {0} %\r\n", (int)(Efficiency * 100)));
            stats.Append(string.Format("Total Heating: {0} HU/t\r\n", TotalHeatPerTick));
            stats.Append(string.Format("Total Cooling: {0} HU/t\r\n", TotalCoolingPerTick));
            stats.Append(string.Format("Net Heating: {0} HU/t\r\n", NetHeatingRate));
            stats.Append(string.Format("Heat Multiplier: {0} %\r\n\r\n",(int)(HeatMultiplier*100)));

            return stats.ToString();
        }
    }
}
