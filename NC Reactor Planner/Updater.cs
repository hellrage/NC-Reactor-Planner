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
        public static async Task<Tuple<bool,Version,string>> CheckForUpdateAsync()
        {
            string GitAPI = "https://api.github.com/repos/hellrage/NC-Reactor-Planner/git/refs/tags?access_token=f83a69064bb51dd50b5b78cafdfa5433910710a2";
            WebResponse webResponse;
            try
            {
                webResponse = await GetWebResponseAsync(GitAPI);
            }
            catch(Exception e)
            {
                MessageBox.Show("An exception occured when checking for updates:\r\n" + e.Message);
                return Tuple.Create(false, Reactor.saveVersion, "");
            }
            
            string responseJSON;
            using (StreamReader r = new StreamReader(webResponse.GetResponseStream()))
            {
                responseJSON = r.ReadToEnd();
            }

            Tuple<Version,string> release = await FindLatest(responseJSON, 2);
            if (release.Item1 > Reactor.saveVersion)
                return Tuple.Create(true, release.Item1,release.Item2);
            else
                return Tuple.Create(false, Reactor.saveVersion, "");
        }

        private static async Task<string> GetCommitMessage(string url)
        {
            WebResponse response = await GetWebResponseAsync(url);
            string responseJSON;
            using (StreamReader r = new StreamReader(response.GetResponseStream()))
            {
                responseJSON = r.ReadToEnd();
            }
            using (JsonTextReader reader = new JsonTextReader(new StringReader(responseJSON)))
            {
                while (reader.Read())
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "message")
                    {
                        reader.Read();
                        return reader.Value.ToString();
                    }
            }
            return "";
        }

        public static async void PerformFullUpdate(Version version, string fileName)
        {
            await DownloadVersionAsync(version, fileName);
            FileInfo updatedExe = new FileInfo(fileName);
            FileInfo tempSave = new FileInfo(updatedExe.DirectoryName + ((Reactor.UI.LoadedSaveFile != null) ? "\\" + Reactor.UI.LoadedSaveFile.Name : "\\temp.json"));
            Reactor.Save(tempSave);
            string oldFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            System.Diagnostics.Process.Start(new FileInfo(fileName).FullName, string.Format("-finalizeupdate \"{0}\" \"{1}\"", tempSave.FullName, oldFile));
            Reactor.UI.Close();
            Application.Exit();
        }

        public static async Task<bool> DownloadVersionAsync(Version version, string fileName)
        {
            string DLLink = FormDLLLink(version);
            WebResponse response = await GetWebResponseAsync(DLLink);

            FileInfo updatedPlanner = new FileInfo(fileName);
            using (var writer = updatedPlanner.OpenWrite())
            {
                await response.GetResponseStream().CopyToAsync(writer);
            }
            return true;
        }

        private static async Task<WebResponse> GetWebResponseAsync(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.UserAgent = "NC-Reactor-Planner-App";
            return await webRequest.GetResponseAsync();
        }

        private static async Task<Tuple<Version,string>> FindLatest(string json, int major)
        {
            Tuple<Version,string> latest = Tuple.Create(new Version(major, 0, 0, 0), "");
            using (JsonTextReader reader = new JsonTextReader(new StringReader(json)))
            {
                while (reader.Read())
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "ref")
                    {
                        reader.Read();
                        string vstr = reader.Value.ToString().Split('v')[1];
                        string[] numbers = vstr.Split('.');
                        Version version = new Version(Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]), Convert.ToInt32(numbers[2]), 0);
                        if(version.Major == major && version > latest.Item1)
                        {
                            do
                            {
                                reader.Read();
                            } while(reader.TokenType != JsonToken.StartObject);
                            do
                            {
                                reader.Read();
                            } while (reader.Value.ToString() != "url");
                            reader.Read();
                            string message = await GetCommitMessage(reader.Value.ToString()+"?access_token=f83a69064bb51dd50b5b78cafdfa5433910710a2");
                            latest = Tuple.Create(version, message);
                        }
                    }
            }

            return latest;
        }

        private static string FormDLLLink(Version rV)
        {
            return "https://github.com/hellrage/NC-Reactor-Planner/releases/download/v" + ShortVersionString(rV) + "/" + ExecutableName(rV);
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
