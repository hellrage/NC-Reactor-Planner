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
        public double TotalOutput { get; private set; }
        public double NetHeatingRate { get => (TotalHeatPerTick - TotalCoolingPerTick); }
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
            double coolingPenaltyMultiplier = 0;
            if(TotalCoolingPerTick > 0 & TotalHeatPerTick > 0)
                coolingPenaltyMultiplier = Math.Min(1, TotalHeatPerTick / TotalCoolingPerTick);

            Efficiency = rawEfficiency * coolingPenaltyMultiplier;
            TotalOutput *= coolingPenaltyMultiplier;

            if (TotalCoolingPerTick > 0 & TotalHeatPerTick > 0)
                FuelDurationMultiplier = Math.Min(1, TotalCoolingPerTick / TotalHeatPerTick);
            else
                FuelDurationMultiplier = 0;
            HeatMultiplier = sumHeatMulti / fuelCells.Count;
        }

        public string GetStatString()
        {
            if (!Valid)
                return string.Format("Cluster №{0}\r\nInvalid! Skipping.\r\n\r\n", ID);
            else return string.Format("Cluster №{6}\r\n" +
                                "Total output: {0}\r\n" +
                                "Efficiency: {1} %\r\n" +
                                "Total Heating: {2} HU/t\r\n" +
                                "Total Cooling: {3} HU/t\r\n" +
                                "Net Heating: {4} HU/t\r\n" +
                                "Heat Multiplier: {5} %\r\n\r\n", TotalOutput, (int)(Efficiency*100), TotalHeatPerTick, TotalCoolingPerTick, NetHeatingRate, (int)(HeatMultiplier*100), ID);
        }
    }
}
