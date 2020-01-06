using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

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
        public Dictionary<string, List<Vector3>> HeatSinks { get; private set; }
        public Dictionary<string, List<Vector3>> Moderators { get; private set; }
        public List<Vector3> Conductors { get; private set; }
        public List<Vector3> Reflectors { get; private set; }
        public Dictionary<string, List<Vector3>> FuelCells { get; private set; }
        public Vector3 InteriorDimensions { get; private set; }
        public string CoolantRecipeName { get; private set; }

        [Newtonsoft.Json.JsonConstructor]
        public SaveData(Version saveVersion, Dictionary<string, List<Vector3>> heatSinks, Dictionary<string, List<Vector3>> moderators, List<Vector3> conductors, List<Vector3> reflectors, Dictionary<string, List<Vector3>> fuelCells, Vector3 interiorDimensions, string coolantRecipeName)
        {
            SaveVersion = saveVersion;
            HeatSinks = heatSinks;
            Moderators = moderators;
            Conductors = conductors;
            Reflectors = reflectors;
            FuelCells = fuelCells;
            InteriorDimensions = interiorDimensions;
            CoolantRecipeName = coolantRecipeName;
        }

        public ValidationResult PerformValidation()
        {
            if (SaveVersion < new Version(2, 0, 0, 0))
                return new ValidationResult(false, "Pre-overhaul savefiles not supported!");

            if (SaveVersion == new Version(2, 0, 0, 0))
            {
                Dictionary<string, List<Vector3>> ValidatedFuelCells = new Dictionary<string, List<Vector3>>();
                foreach (KeyValuePair<string, List<Vector3>> fuelCellGroup in FuelCells)
                {
                    List<string> props = fuelCellGroup.Key.Split(';').ToList();

                    switch (props.Count)
                    {
                        case 0:
                        case 1:
                            return new ValidationResult(false, "Tried to load an invalid FuelCell: " + fuelCellGroup.Key);
                        case 2:
                            string newFuelName = "[OX]" + props[0].Replace(" Oxide", "");
                            if (Palette.FuelPalette.ContainsKey(newFuelName))
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
            else if (SaveVersion < new Version(2, 0, 30))
            {
                Dictionary<string, List<Vector3>> ValidatedFuelCells = new Dictionary<string, List<Vector3>>();
                foreach (var fuelCellGroup in FuelCells)
                {
                    string newKey = fuelCellGroup.Key + (fuelCellGroup.Key.Contains(";True") ? ";Cf-252" : ";None");
                    ValidatedFuelCells.Add(newKey, fuelCellGroup.Value);
                }
                FuelCells = ValidatedFuelCells;
            }

            if(SaveVersion < new Version(2, 0, 6, 0))
            {
                Reflectors = new List<Vector3>();
            }

            if(CoolantRecipeName == null || !Configuration.CoolantRecipes.ContainsKey(CoolantRecipeName))
            {
                CoolantRecipeName = Configuration.CoolantRecipes.First().Key;
                return new ValidationResult(true, "No such coolant recipe in the configuration! Reset to first available recipe.");
            }
            return new ValidationResult(true, "Valid savefile.");
        }
    }
}
