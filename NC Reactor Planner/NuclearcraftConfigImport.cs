using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public class NuclearcraftConfigImport
    {
        private string _blockName;
        private string _variableType;
        private string _variableName;
        private string _currentValue;
        private List<string> _listValues;

        private enum ParseState
        {
            Start,
            Block,
            Item,
            List
        }

        public static NuclearcraftConfigImport ImportConfig(FileInfo file)
        {
            if (!file.Exists)
            {
                return null;
            }

            NuclearcraftConfigImport importer = new NuclearcraftConfigImport();
            importer.ParseConfig(file);

            return importer;
        }

        private Dictionary<string, Dictionary<string, object>> configurationValues = new Dictionary<string, Dictionary<string, object>>();

        public bool HasBlock(string block)
        {
            return configurationValues.ContainsKey(block);
        }

        public T Get<T>(string block, string key) {
            return (T)configurationValues[block][key];
        }

        public T GetItem<T>(string block, string key, int item)
        {
            return ((List<T>)configurationValues[block][key])[item];
        }

        public string LastError { get; set; }

        private void ParseConfig(FileInfo file)
        {
            ParseState state = ParseState.Start;
            using (StreamReader sr = new StreamReader(file.OpenRead()))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    line = line.Trim(' ', '\t');
                    if (line[0] == '#')
                        continue;

                    DoStep(line, ref state);
                }
            }
        }

        Regex blockstart = new Regex(@"(\w+)\s*{", RegexOptions.IgnoreCase);
        Regex blockend = new Regex(@"\s*}");
        Regex simplevariable = new Regex(@"(\w):(\w+)\s*=\s*([\w.-]+)");
        Regex listvariable = new Regex(@"(\w):(\w+)\s*<");
        Regex listitem = new Regex(@"([\w.-]+)");
        Regex listend = new Regex(@">");

        private void DoStep(string line, ref ParseState state)
        {
            if (state == ParseState.Start)
            {
                // We're looking for a opening block:
                // We're specifically looking for "<identifier> { [# anything]"
                var match = blockstart.Match(line);
                if (match != null && match.Success)
                {
                    // We're starting a block
                    _blockName = match.Groups[1].Value;
                    state = ParseState.Block;
                    return;
                }
                else
                {
                    // Could not find the string? wha?
                    if (string.IsNullOrWhiteSpace(LastError))
                        LastError = string.Empty;
                    LastError += "Could not find the start of the next block?" + Environment.NewLine;
                    return;
                }
            }
            else if (state == ParseState.Block)
            {
                if (blockend.IsMatch(line))
                {
                    _blockName = string.Empty;

                    state = ParseState.Start;
                    return;
                }

                var simple = simplevariable.Match(line);
                if (simple.Success)
                {
                    _variableType = simple.Groups[1].Value;
                    _variableName = simple.Groups[2].Value;
                    _currentValue = simple.Groups[3].Value;

                    AddConfigVariable(_blockName, _variableType, _variableName, _currentValue);

                    _variableType = _variableName = _currentValue = null;
                    return;
                }

                var list = listvariable.Match(line);
                if (list.Success)
                {
                    _variableType = list.Groups[1].Value;
                    _variableName = list.Groups[2].Value;
                    _listValues = new List<string>();

                    state = ParseState.List;
                    return;
                }

                if (string.IsNullOrWhiteSpace(LastError))
                    LastError = string.Empty;
                LastError += $"Unknown token, looking for the end of a block, or a simple variable, or a list variable? {line}" + Environment.NewLine;
            }
            else if (state == ParseState.List)
            {
                if (listend.IsMatch(line))
                {
                    AddConfigVariableList(_blockName, _variableType, _variableName, _listValues);
                    _variableType = _variableName = _currentValue = null;
                    _listValues = null;

                    state = ParseState.Block;
                    return;
                }

                var item = listitem.Match(line);
                if (item.Success)
                {
                    _listValues.Add(item.Groups[1].Value);
                    return;
                }

                if (string.IsNullOrWhiteSpace(LastError))
                    LastError = string.Empty;
                LastError += $"Unknown token, looking for the end of a list, or a list item? {line}" + Environment.NewLine;
            }
        }

        private void AddConfigVariable(string block, string type, string name, string value)
        {
            if (!configurationValues.ContainsKey(block))
                configurationValues.Add(block, new Dictionary<string, object>());

            if (configurationValues[block].ContainsKey(name))
            {
                if (string.IsNullOrWhiteSpace(LastError))
                    LastError = string.Empty;
                LastError += $"Duplicate configuration key: '{name}' in block '{block}'" + Environment.NewLine;
                return;
            }

            switch (type)
            {
                case "S":
                    configurationValues[block].Add(name, value);
                    break;
                case "I":
                    configurationValues[block].Add(name, int.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
                    break;
                case "D":
                    configurationValues[block].Add(name, double.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
                    break;
                case "B":
                    configurationValues[block].Add(name, bool.Parse(value));
                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    configurationValues[block].Add(name, value);
                    break;
            }
        }

        private void AddConfigVariableList(string block, string type, string name, List<string> value)
        {
            if (!configurationValues.ContainsKey(block))
                configurationValues.Add(block, new Dictionary<string, object>());

            if (configurationValues[block].ContainsKey(name))
            {
                if (string.IsNullOrWhiteSpace(LastError))
                    LastError = string.Empty;
                LastError += $"Duplicate configuration key: '{name}' in block '{block}'" + Environment.NewLine;
                return;
            }

            switch (type)
            {
                case "S":
                    configurationValues[block].Add(name, value);
                    break;
                case "I":
                    configurationValues[block].Add(name, value.Select(x => int.Parse(x, System.Globalization.CultureInfo.InvariantCulture)).ToList());
                    break;
                case "D":
                    configurationValues[block].Add(name, value.Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture)).ToList());
                    break;
                case "B":
                    configurationValues[block].Add(name, value.Select(x => bool.Parse(x)).ToList());
                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    configurationValues[block].Add(name, value);
                    break;
            }
        }

    }
}
