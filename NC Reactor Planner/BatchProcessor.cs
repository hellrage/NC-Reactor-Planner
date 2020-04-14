using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NC_Reactor_Planner
{
    public static class BatchProcessor
    {
        private static DirectoryInfo outputFolder;
        public static void Process(DirectoryInfo folder)
        {
            FileInfo[] saveFiles = folder.GetFiles("*.json");
            outputFolder = new DirectoryInfo(folder.FullName + "\\output");
            Directory.CreateDirectory(outputFolder.FullName);

            using (TextWriter tw = File.CreateText(outputFolder.FullName + "\\Info "+DateTime.Now.ToShortTimeString().Replace(":","-")+".txt"))
            using (TextWriter csvw = File.CreateText(outputFolder.FullName + "\\Stats " + DateTime.Now.ToShortTimeString().Replace(":", "-") + ".csv"))
            {
                csvw.WriteLine("fileName,totalOutput,totalHeatPerTick,totalCoolingPerTick;");
                foreach (var saveFile in saveFiles)
                {
                    tw.WriteLine("Processing file: " + saveFile.Name);
                    ValidationResult vr = Reactor.Load(saveFile.OpenText().ReadToEnd(), true, saveFile.FullName);
                    if (!vr.Successful)
                    {
                        tw.WriteLine("\tFailed to load savefile! Skipping");
                        continue;
                    }
                    tw.WriteLine("\tReactor loaded");
                    Reactor.Update();
                    tw.WriteLine("\tReactor updated");
                    Reactor.SaveReactorAsImage(folder + "\\output\\" + saveFile.Name.Replace("json", "png"), Reactor.UI.StatLineCount);
                    tw.WriteLine("\tPng exported");
                    csvw.WriteLine(String.Format("{0},{1},{2},{3};", saveFile.Name, Math.Round(Reactor.totalOutputPerTick,0), Reactor.totalHeatPerTick, Reactor.totalCoolingPerTick));
                    tw.WriteLine("\tCSV written");
                    tw.WriteLine("\tDone with " + saveFile.Name);
                }
                tw.Flush();
                csvw.Flush();
            }
        }
    }
}
