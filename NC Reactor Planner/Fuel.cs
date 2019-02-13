using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public class Fuel
    {
        private string _name;
        //private string _saveSafeName;

        private double _baseHeat;
        private double _fuelTime;
        private double _baseEfficiency;
        private double _criticalityFactor;

        public string Name { get => _name; private set => _name = value; }
        public double BaseHeat { get => _baseHeat; private set => _baseHeat = value; }
        public double FuelTime { get => _fuelTime; private set => _fuelTime = value; }
        public double BaseEfficiency { get => _baseEfficiency; private set => _baseEfficiency = value; }
        public double CriticalityFactor { get => _criticalityFactor; private set => _criticalityFactor = value; }

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
            return Name;
        }

        public void ReloadValuesFromConfig()
        {
            FuelValues fv = Configuration.Fuels[Name];
            BaseHeat = fv.BaseHeat;
            FuelTime = fv.FuelTime;
        }
    }
}
