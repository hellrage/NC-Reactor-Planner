using System;
using System.Collections.Generic;

namespace NC_Reactor_Planner
{
    public class Fuel
    {
        public string Name { get; private set; }
        public double BasePower { get; private set; }
        public double BaseHeat { get; private set; }
        public double FuelTime { get; private set; }
        
        [Newtonsoft.Json.JsonConstructor]
        public Fuel(string name, double basePower, double baseHeat, double fuelTime)
        {
            Name = name;
            BasePower = basePower;
            BaseHeat = baseHeat;
            FuelTime = fuelTime;
        }

        public override string ToString()
        {
            return Name;
        }

        public void ReloadValuesFromConfig()
        {
            FuelValues fv = Configuration.Fuels[Name];
            BasePower = fv.BasePower;
            BaseHeat = fv.BaseHeat;
            FuelTime = fv.FuelTime;
        }
    }
}
