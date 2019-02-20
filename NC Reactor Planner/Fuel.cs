using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public class Fuel
    {
        public string Name { get; private set; }
        public string Fissile { get; private set; }
        public string Fertile { get; private set; }
        public double BasePower { get; private set; }
        public double BaseHeat { get; private set; }
        public double FuelTime { get; private set; }

        public Fuel(string name, string fissile, string fertile, double basePower, double baseHeat, double fuelTime): this(name, basePower, baseHeat, fuelTime)
        {
            Fissile = fissile;
            Fertile = fertile;
        }

        public Fuel(string name, double basePower, double baseHeat, double fuelTime)
        {
            Name = name;
            BasePower = basePower;
            BaseHeat = baseHeat;
            FuelTime = fuelTime;
        }

        public Fuel()
        {
            //Added for backwards save compatibility
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
