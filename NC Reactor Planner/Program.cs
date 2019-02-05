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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            PreStartUp();
            Application.Run(new PlannerUI());
        }

        static void PreStartUp()
        {
            FileInfo jsonDll = new FileInfo("Newtonsoft.json.dll");
            if (!jsonDll.Exists)
            {
                //MessageBox.Show("You do not have the required dll (Newtonsoft.json.dll) in the application folder, it is a dependency! Please download it from the same mediafire link you got the release from and put it next to the executable. The application will work but it's going to crash when you attempt to save your reactor\\configuration");
                using (var writer = jsonDll.OpenWrite())
                {
                    writer.Write(Resources.Newtonsoft_Json, 0, Resources.Newtonsoft_Json.Length);
                }
            }
            Configuration.ResetToDefaults();
            //Reactor.InitializeReactor(1, 1, 1);

            FileInfo defaultConfig = new FileInfo("DefaultConfig.json");
            if (!defaultConfig.Exists)
                Configuration.Save(defaultConfig);
            else
                if(!Configuration.Load(defaultConfig))
                {
                    MessageBox.Show("Unable to load default configuration, resetting to hardcoded defaults...");
                    Configuration.ResetToDefaults();
                }
        }
    }
}
