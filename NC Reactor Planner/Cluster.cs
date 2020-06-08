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
        public int MeltdownTime { get; private set; }
        public NetHeatClass NetHeatClass { get; private set; }
        public double HeatMultiplier { get; private set; }
        public bool Valid { get => HasPathToCasing; }
        public int ID { get; private set; }

        public List<Block> Blocks { get; private set; }
        public List<FuelCell> ActiveFuelCells { get; private set; } 

        public Cluster(int id)
        {
            Blocks = new List<Block>();
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
            MeltdownTime = -1;
        }

        public void AddBlock(Block block)
        {
            Blocks.Add(block);
        }

        public void UpdateStats()
        {
            double sumEffOfFuelCells = 0;
            double sumHeatMulti = 0;
            ResetValues();
            ActiveFuelCells = new List<FuelCell>();
            foreach(Block block in Blocks)
            {
                switch (block.BlockType)
                {
                    case BlockTypes.HeatSink:
                        TotalCoolingPerTick += ((HeatSink)block).Cooling;
                        break;
                    case BlockTypes.FuelCell:
                        FuelCell fuelCell = block as FuelCell;
                        ActiveFuelCells.Add(fuelCell);

                        if (!fuelCell.Active)
                            continue;

                        TotalHeatPerTick += fuelCell.HeatProducedPerTick;
                        TotalOutput += fuelCell.Efficiency * fuelCell.UsedFuel.BaseHeat;
                        sumEffOfFuelCells += fuelCell.Efficiency;
                        sumHeatMulti += fuelCell.HeatMultiplier;
                        break;
                    case BlockTypes.Irradiator:
                        Irradiator irradiator = block as Irradiator;
                        TotalHeatPerTick += irradiator.HeatPerTick;
                        break;
                    case BlockTypes.NeutronShield:
                        NeutronShield neutronShield = block as NeutronShield;
                        TotalHeatPerTick += neutronShield.HeatPerTick;
                        break;
                    case BlockTypes.Conductor:
                        Conductor conductor = block as Conductor;
                        conductor.HasPathToCasing |= this.HasPathToCasing;
                        break;
                    case BlockTypes.Moderator:
                    case BlockTypes.Air:
                    case BlockTypes.Casing:
                    default:
                        throw new ArgumentException("Unexpected blockType in cluster: " + block.BlockType.ToString());
                }
            }

            TotalHeatPerTick *= Configuration.Fission.HeatGeneration;
            TotalOutput *= Configuration.Fission.Power;

            double rawEfficiency = 0;
            if (ActiveFuelCells.Count > 0)
                rawEfficiency = sumEffOfFuelCells / ActiveFuelCells.Count;
            if(TotalCoolingPerTick > 0 & TotalHeatPerTick > 0)
                CoolingPenaltyMultiplier = Math.Min(1, (TotalHeatPerTick + Configuration.Fission.CoolingPenaltyLeniency) / TotalCoolingPerTick);

            Efficiency = rawEfficiency * CoolingPenaltyMultiplier;
            TotalOutput *= CoolingPenaltyMultiplier;

            if (TotalCoolingPerTick > 0 & TotalHeatPerTick > 0)
                FuelDurationMultiplier = Math.Min(1, (TotalCoolingPerTick + Configuration.Fission.CoolingPenaltyLeniency) / TotalHeatPerTick);
            else
                FuelDurationMultiplier = 1;
            HeatMultiplier = sumHeatMulti / ActiveFuelCells.Count;

            if (NetHeatingRate < -Configuration.Fission.CoolingPenaltyLeniency)
                NetHeatClass = NetHeatClass.Overcooled;
            else if (NetHeatingRate > Configuration.Fission.CoolingPenaltyLeniency)
                NetHeatClass = NetHeatClass.Overheating;
            else if (NetHeatingRate > 0)
                NetHeatClass = NetHeatClass.HeatPositive;
            else
                NetHeatClass = NetHeatClass.Safe;
        }

        public string GetStatString()
        {
            if (!Valid)
                return string.Format("Cluster №{0}\r\nInvalid! Skipping.\r\n\r\n", ID);
            StringBuilder stats = new StringBuilder();
            stats.AppendLine($"Cluster №{ID}");
            stats.AppendLine($"Total output: {TotalOutput}");
            stats.AppendLine($"Efficiency: {(int)(Efficiency * 100)} %");
            stats.AppendLine($"Total Heating: {TotalHeatPerTick} HU/t");
            stats.AppendLine($"Total Cooling: {TotalCoolingPerTick} HU/t");
            stats.AppendLine($"Net Heating: {NetHeatingRate} HU/t");
            stats.AppendLine($"Heat Multiplier: {(int)(HeatMultiplier * 100)} %");
            stats.AppendLine($"Cooling penalty mult: {Math.Round(CoolingPenaltyMultiplier, 4).ToString()}");
            stats.AppendLine();

            return stats.ToString();
        }
    }

    public enum NetHeatClass
    {
        Safe,
        Overheating,
        Overcooled,
        HeatPositive
    }
}
