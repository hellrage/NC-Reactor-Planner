using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NC_Reactor_Planner
{
    public class Fuel : IComparable<Fuel>
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

		private string getModifier()
		{
			Match match = Regex.Match(Name, @"\[([a-zA-Z]+)\]");
			if (match.Success)
				return match.Groups[0].Value;
			else
				return "";
		}

		private int getIsotope()
		{
			Match match = Regex.Match(Name, @"-([0-9]+)");
			if (match.Success)
				return Int32.Parse(match.Groups[0].Value);
			else
				return -1;
		}

		private string getElement()
		{
			Match match = Regex.Match(Name, @"([a-zA-Z]+)(?!\])\b");
			if (match.Success)
				return match.Groups[0].Value;
			else
				return "";
		}

		public int CompareTo(Fuel right)
		{
			if (right == null)
				return 1;

			int leftIsotope = getIsotope();
			int rightIsotope = right.getIsotope();
			int compareIsotope = leftIsotope.CompareTo(rightIsotope);
			if (compareIsotope != 0)
				return compareIsotope;

			string leftElement = getElement();
			string rightElement = right.getElement();
			int compareElement = leftElement.CompareTo(rightElement);
			if (compareIsotope != 0)
				return compareIsotope;

			string leftModifier = getModifier();
			string rightModifier = right.getModifier();
			int compareModifier = leftModifier.CompareTo(rightModifier);

			return compareModifier;
		}
	}
}
