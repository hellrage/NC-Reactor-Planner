using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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

            FileInfo jsonDll = new FileInfo("Newtonsoft.json.dll");
            if(!jsonDll.Exists)
                MessageBox.Show("You do not have the required dll (Newtonsoft.json.dll) in the application folder, it is a dependency! Please download it from the same mediafire link you got the release from and put it next to the executable. The application will work but it's going to crash when you attempt to save your reactor\\configuration");

            Properties.Settings.Default.Upgrade();
            Configuration.ResetToDefaults();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PlannerUI());
        }
    }
}
