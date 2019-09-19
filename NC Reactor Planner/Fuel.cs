using System;
using System.Collections.Generic;

namespace NC_Reactor_Planner
{
    public class Fuel
    {
        public string Name { get ; private set; }
        public double BaseHeat { get; private set; }
        public double FuelTime { get; private set; }
        public double BaseEfficiency { get; private set; }
        public int CriticalityFactor { get; private set; }

        public Fuel(string name, double baseEfficiency, double baseHeat, double fuelTime, int criticalityFactor)
        {
            Name = name;
            BaseHeat = baseHeat;
            FuelTime = fuelTime;
            CriticalityFactor = criticalityFactor;
            BaseEfficiency = baseEfficiency;
        }

        public Fuel()
        {
            //Added for backwards save compatibility
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}", Name.PadRight(14), BaseEfficiency.ToString().PadRight(6), BaseHeat.ToString().PadRight(5), CriticalityFactor.ToString().PadRight(4));
        }

        public void ReloadValuesFromConfig()
        {
            FuelValues fv = Configuration.Fuels[Name];
            BaseHeat = fv.BaseHeat;
            FuelTime = fv.FuelTime;
            BaseEfficiency = fv.BaseEfficiency;
            CriticalityFactor = fv.CriticalityFactor;
        }
    }
}
