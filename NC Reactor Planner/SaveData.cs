using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public struct ValidationResult
    {
        public bool Successful;
        public string Result;

        public ValidationResult(bool successful, string result)
        {
            Successful = successful;
            Result = result;
        }
    }

    public class SaveData
    {
        public Version SaveVersion { get; private set; }
        public Dictionary<string, List<Point3D>> HeatSinks { get; private set; }
        public Dictionary<string, List<Point3D>> Moderators { get; private set; }
        public List<Point3D> Conductors { get; private set; }
        public List<Point3D> Reflectors { get; private set; }
        public Dictionary<string, List<Point3D>> FuelCells { get; private set; }
        public Size3D InteriorDimensions { get; private set; }

        public SaveData(Version saveVersion, Dictionary<string, List<Point3D>> heatSinks, Dictionary<string, List<Point3D>> moderators, List<Point3D> conductors, List<Point3D> reflectors, Dictionary<string, List<Point3D>> fuelCells, Size3D interiorDimensions)
        {
            SaveVersion = saveVersion;
            HeatSinks = heatSinks;
            Moderators = moderators;
            Conductors = conductors;
            Reflectors = reflectors;
            FuelCells = fuelCells;
            InteriorDimensions = interiorDimensions;
        }

        public ValidationResult PerformValidation()
        {
            if (SaveVersion < new Version(2, 0, 0, 0))
                return new ValidationResult(false, "Pre-overhaul savefiles not supported!");
            if (SaveVersion == new Version(2, 0, 0, 0))
            {
                Dictionary<string, List<Point3D>> ValidatedFuelCells = new Dictionary<string, List<Point3D>>();
                foreach (KeyValuePair<string, List<Point3D>> fuelCellGroup in FuelCells)
                {
                    List<string> props = fuelCellGroup.Key.Split(';').ToList();

                    switch (props.Count)
                    {
                        case 0:
                        case 1:
                            return new ValidationResult(false, "Tried to load an invalid FuelCell: " + fuelCellGroup.Key);
                        case 2:
                            string newFuelName = "[OX]" + props[0].Replace(" Oxide", "");
                            Fuel usedFuel;
                            if (Palette.FuelPalette.TryGetValue(newFuelName, out usedFuel))
                                ValidatedFuelCells.Add(string.Join(";", newFuelName, props[1]), fuelCellGroup.Value);
                            else
                                ValidatedFuelCells.Add(string.Join(";", Palette.FuelPalette.First().Value.Name, props[1]), fuelCellGroup.Value);
                            break;
                        default:
                            return new ValidationResult(false, "Tried to load an unexpected FuelCell: " + fuelCellGroup.Key);
                    }
                }
                FuelCells = ValidatedFuelCells;
            }
            if(SaveVersion < new Version(2, 0, 6, 0))
            {
                Reflectors = new List<Point3D>();
            }

            return new ValidationResult(true, "Valid savefile.");
        }
    }
}
