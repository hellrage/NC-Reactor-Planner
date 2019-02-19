using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public static class Updater
    {
        public static async Task<Tuple<bool,Version>> CheckForUpdateAsync()
        {
            string GitAPI = "https://api.github.com/repos/hellrage/NC-Reactor-Planner/git/refs/tags";
            
            WebResponse webResponse = await GetWebResponseAsync(GitAPI);
            
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
            if (releaseVersion >= Reactor.saveVersion)
                return Tuple.Create(true, releaseVersion);
            else
                return Tuple.Create(false, Reactor.saveVersion);
        }

        public static async void DownloadVersionAsync(Version version, string fileName)
        {
            string DLLink = FormDLLLink(version);
            WebResponse response = await GetWebResponseAsync(DLLink);

            FileInfo updatedPlanner = new FileInfo(fileName);
            using (var writer = updatedPlanner.OpenWrite())
            {
                await response.GetResponseStream().CopyToAsync(writer);
            }
            MessageBox.Show(string.Format("Downloaded {0}", updatedPlanner.FullName));
        }

        private static async Task<WebResponse> GetWebResponseAsync(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
            webRequest.UserAgent = "NC-Reactor-Planner-App";
            return await webRequest.GetResponseAsync();
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
            return "https://github.com/hellrage/NC-Reactor-Planner/releases/download/v"+ShortVersionString(rV)+"/" + ExecutableName(rV);
        }

        public static string ShortVersionString(Version v)
        {
            return string.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);
        }

        public static string ExecutableName(Version v)
        {
            return "NC.Reactor.Planner." + ShortVersionString(v) + ".exe";
        }
    }
}
