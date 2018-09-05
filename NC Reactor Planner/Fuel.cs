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
        private string _saveSafeName;
        private string _fissile;
        private string _fertile;
        private double _basePower;
        private double _baseHeat;
        private int _fuelTime;

        public string Name { get => _name; private set => _name = value; }
        public string saveSafeName { get => _saveSafeName; private set => _saveSafeName = value; }
        public string Fissile { get => _fissile; private set => _fissile = value; }
        public string Fertile { get => _fertile; private set => _fertile = value; }
        public double BasePower { get => _basePower; private set => _basePower = value; }
        public double BaseHeat { get => _baseHeat; private set => _baseHeat = value; }
        public int FuelTime { get => _fuelTime; private set => _fuelTime = value; }

        public Fuel(string name, string fissile, string fertile, double basePower, double baseHeat, int fuelTime)
        {
            Name = name;
            saveSafeName = Name.Replace(" ", "__").Replace("-", "_");
            Fissile = fissile;
            Fertile = fertile;
            BasePower = basePower;
            BaseHeat = baseHeat;
            FuelTime = fuelTime;
        }

        public override string ToString()
        {
            return Name;
        }

        public void ReloadValuesFromSettings()
        {
            List<string> values = ModValueSettings.RetrieveSplitSettings(saveSafeName);
            try
            {
                BasePower = Convert.ToDouble((values[0]));
                BaseHeat = Convert.ToDouble((values[1]));
                FuelTime = Convert.ToInt32((values[2]));
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + string.Format("\r\n There were invalid values in the settings: {0}: {1} {2} {3}\r\n Leaving the values as default.", Name, values[0], values[1], values[2]));
            }
        }
    }
}
