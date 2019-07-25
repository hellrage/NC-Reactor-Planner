using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NC_Reactor_Planner.Properties;

namespace NC_Reactor_Planner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            PreStartUp();
            if (args.Length > 1)
            {
                switch (args[0])
                {
                    case "-finalizeupdate":
                        AfterUpdate(args[2], args[1]);
                        Application.Run(Reactor.UI);
                        break;
                    case "-batch":
                        BatchProcessor.Process(new DirectoryInfo(args[1]));
                        System.Environment.Exit(0);
                        break;
                    default:
                        if (File.Exists(args[0]))
                            AfterUpdate(args[1], args[0]);
                        break;
                }
            }
            else
                Application.Run(Reactor.UI);
        }

        static void PreStartUp()
        {
            FileInfo jsonDll = new FileInfo("Newtonsoft.json.dll");
            if (!jsonDll.Exists)
            {
                using (var writer = jsonDll.OpenWrite())
                {
                    writer.Write(Resources.Newtonsoft_Json, 0, Resources.Newtonsoft_Json.Length);
                }
            }

            FileInfo defaultConfig = new FileInfo("BetaConfig.json");
            if (!defaultConfig.Exists)
            {
                Configuration.ResetToDefaults();
                Configuration.Save(defaultConfig);
            }
            else if (!Configuration.Load(defaultConfig))
                Configuration.ResetToDefaults();
        }

        static void AfterUpdate(string exePath, string savePath)
        {
            Reactor.Load(new FileInfo(savePath));
            File.Delete(exePath);
        }
    }
}
