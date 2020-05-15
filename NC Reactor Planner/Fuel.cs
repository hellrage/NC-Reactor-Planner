using System;
using System.Collections.Generic;

namespace NC_Reactor_Planner
{
    public class Fuel
    {
        public string Name { get ; private set; }
        public double BaseHeat { get => Configuration.Fuels[Name].BaseHeat; }
        public double FuelTime { get => Configuration.Fuels[Name].FuelTime; }
        public double BaseEfficiency { get => Configuration.Fuels[Name].BaseEfficiency; }
        public int CriticalityFactor { get => Configuration.Fuels[Name].CriticalityFactor; }
        public bool SelfPriming { get => Configuration.Fuels[Name].SelfPriming; }

        public Fuel(string name)
        {
            Name = name;
        }

        public Fuel()
        {
            //Added for backwards save compatibility
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}", (Name + (SelfPriming ? "*" : "")).PadRight(14), BaseEfficiency.ToString().PadRight(6), BaseHeat.ToString().PadRight(5), CriticalityFactor.ToString().PadRight(4));
        }
    }
}
