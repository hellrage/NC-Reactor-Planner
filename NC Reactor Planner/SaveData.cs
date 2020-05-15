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
        public Dictionary<string, object> Data { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        public SaveData(Version saveVersion, Dictionary<string, object> data)
        {
            SaveVersion = saveVersion;
            Data = data;
        }

        public Dictionary<string, List<Vector3>> DataDictionary(string key)
        {
            object result;
            Data.TryGetValue(key, out result);
            return (result as Dictionary<string, List<Vector3>>) ?? new Dictionary<string, List<Vector3>>();
        }

        public List<Vector3> DataList(string key)
        {
            object result;
            Data.TryGetValue(key, out result);
            return (result as List<Vector3>) ?? new List<Vector3>();
        }

        public string DataStringOrDefault(string key, string defaultValue)
        {
            object result;
            Data.TryGetValue(key, out result);
            return (result as string) ?? defaultValue;
        }
    }
}
