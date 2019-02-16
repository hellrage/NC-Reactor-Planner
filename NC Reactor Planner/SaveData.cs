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
        public Dictionary<string, List<Point3D>> FuelCells { get; private set; }
        public Size3D InteriorDimensions { get; private set; }

        public SaveData(Version saveVersion, Dictionary<string, List<Point3D>> heatSinks, Dictionary<string, List<Point3D>> moderators, List<Point3D> conductors, Dictionary<string, List<Point3D>> fuelCells, Size3D interiorDimensions)
        {
            SaveVersion = saveVersion;
            HeatSinks = heatSinks;
            Moderators = moderators;
            Conductors = conductors;
            FuelCells = fuelCells;
            InteriorDimensions = interiorDimensions;
        }

        public ValidationResult PerformValidation()
        {
            if (SaveVersion < new Version(2, 0, 0))
                return new ValidationResult(false, "Pre-overhaul savefiles not supported!");

            return new ValidationResult(true, "Valid savefile.");
        }
    }
}
