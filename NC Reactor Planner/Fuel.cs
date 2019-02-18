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
        public double FluxMultiplier { get; private set; }

        public Fuel(string name, double baseEfficiency, double baseHeat, double fuelTime, double criticalityFactor, double fluxMultiplier)
        {
            Name = name;
            BaseHeat = baseHeat;
            FuelTime = fuelTime;
            CriticalityFactor = criticalityFactor;
            BaseEfficiency = baseEfficiency;
            FluxMultiplier = fluxMultiplier;
        }

        public Fuel()
        {
            //Added for backwards save compatibility
        }

        public override string ToString()
        {
            return string.Format("{0}/ {1}/ {2}/ {3}/ {4}", Name, BaseEfficiency, BaseHeat, CriticalityFactor, FluxMultiplier);
        }

        public void ReloadValuesFromConfig()
        {
            FuelValues fv = Configuration.Fuels[Name];
            BaseHeat = fv.BaseHeat;
            FuelTime = fv.FuelTime;
        }
    }
}
