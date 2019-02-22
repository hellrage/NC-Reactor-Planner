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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            PreStartUp();
            if (args.Length > 1)
                AfterUpdate(args);
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
            Configuration.ResetToDefaults();

            Palette.Load();

            FileInfo defaultConfig = new FileInfo("BetaConfig.json");
            if (!defaultConfig.Exists)
                Configuration.Save(defaultConfig);
            else
                if(!Configuration.Load(defaultConfig))
                {
                    MessageBox.Show("Unable to load default configuration, resetting to hardcoded defaults...");
                    Configuration.ResetToDefaults();
                }
        }

        static void AfterUpdate(string[] args)
        {
            Reactor.Load(new FileInfo(args[0]));
            File.Delete(args[1]);
        }
    }
}
