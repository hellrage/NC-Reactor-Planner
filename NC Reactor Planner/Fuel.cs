using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public class Fuel
    {
        public string Name { get ; private set; }
        public double BaseHeat { get; private set; }
        public double FuelTime { get; private set; }
        public double BaseEfficiency { get; private set; }
        public double CriticalityFactor { get; private set; }

        public Fuel(string name, double baseEfficiency, double baseHeat, double fuelTime, double criticalityFactor)
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
            return string.Format("{0}/ {1}/ {2}/ {3}", Name, BaseEfficiency, BaseHeat, CriticalityFactor);
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
