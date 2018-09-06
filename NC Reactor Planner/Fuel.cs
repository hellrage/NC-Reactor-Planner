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
        private string _fissile;
        private string _fertile;
        private double _basePower;
        private double _baseHeat;
        private double _fuelTime;

        public string Name { get => _name; private set => _name = value; }
        public string Fissile { get => _fissile; private set => _fissile = value; }
        public string Fertile { get => _fertile; private set => _fertile = value; }
        public double BasePower { get => _basePower; private set => _basePower = value; }
        public double BaseHeat { get => _baseHeat; private set => _baseHeat = value; }
        public double FuelTime { get => _fuelTime; private set => _fuelTime = value; }

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
