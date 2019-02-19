using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace NC_Reactor_Planner
{
    public static class Updater
    {
        public static Tuple<bool,Version> CheckForUpdate()
        {
            string GitAPI = "https://api.github.com/repos/hellrage/NC-Reactor-Planner/git/refs/tags";
            
            WebResponse webResponse = GetWebResponse(GitAPI);
            
            string responseJSON;
            using (StreamReader r = new StreamReader(webResponse.GetResponseStream()))
            {
                responseJSON = r.ReadToEnd();
            }

            List<string> versionTags = new List<string>();
            using (JsonTextReader reader = new JsonTextReader(new StringReader(responseJSON)))
            {
                
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "ref")
                    {
                        reader.Read();
                        string str = reader.Value.ToString();
                        versionTags.Add(str.Split('v')[1]);
                    }
                }

            }
            Version releaseVersion = FindLatest(versionTags, 2);
            if (releaseVersion > Reactor.saveVersion)
                return Tuple.Create(true, releaseVersion);
            else
                return Tuple.Create(false, Reactor.saveVersion);
        }

        public static void DownloadVersion(Version version)
        {
            string DLLink = FormDLLLink(version);
            WebResponse response = GetWebResponse(DLLink);

            FileInfo updatedPlanner = new FileInfo(DLLink.Split('/')[8]);
            using (var writer = updatedPlanner.OpenWrite())
            {
                response.GetResponseStream().CopyTo(writer);
            }
            MessageBox.Show(string.Format("Downloaded {0}\r\nLocation: {1}", updatedPlanner.Name, updatedPlanner.FullName));
        }

        private static WebResponse GetWebResponse(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
            webRequest.UserAgent = "NC-Reactor-Planner-App";
            return webRequest.GetResponseAsync().Result;
        }

        private static Version FindLatest(List<string> versionTags, int major)
        {
            Version latest = new Version(major, 0, 0, 0);
            foreach (string tag in versionTags)
            {
                string[] numbers = tag.Split('.');
                Version version = new Version(Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]), Convert.ToInt32(numbers[2]), 0);
                if (version.Major == major && version > latest)
                    latest = version;
            }
            return latest;
        }

        private static string FormDLLLink(Version rV)
        {
            string version = string.Join(".", rV.Major, rV.Minor, rV.Build);
            return "https://github.com/hellrage/NC-Reactor-Planner/releases/download/v"+version+"/NC.Reactor.Planner."+version+".exe";
        }
    }
}
