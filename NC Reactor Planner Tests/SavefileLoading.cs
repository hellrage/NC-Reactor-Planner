using Microsoft.VisualStudio.TestTools.UnitTesting;
using NC_Reactor_Planner;
using System;
using System.IO;

namespace NC_Reactor_Planner_Tests
{
    [TestClass]
    public class SavefileLoading
    {
        private static string SavefileDirectory;

        [ClassInitialize]
        static public void SetupSaveFileDirectory(TestContext context)
        {
            SavefileDirectory = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, "Savefiles");
        }

        [TestMethod]
        public void Load_Given200Savefile_Completes()
        {
            FileInfo saveFile = new FileInfo(Path.Combine(SavefileDirectory, "200.json"));
            
        }
    }
}
