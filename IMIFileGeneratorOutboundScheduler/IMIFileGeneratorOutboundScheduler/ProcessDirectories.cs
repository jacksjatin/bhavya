using IMIFileGeneratorOutboundScheduler.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace IMIFileGeneratorOutboundScheduler
{

    public class DPKMap
    {
        public string strDpkValue;
        public List<ProcessKey> lstProcessKeys;
        public string imagePath;
    }

    public class ProcessKey
    {
        public string dpkKey;
        public string dpkPostion;
        public string Actualvalue;
    }

    public class ProcessDirectories
    {

        private HelperClasses.Helpers helpers;

        public ProcessDirectories()
        {
            helpers = new HelperClasses.Helpers();
        }
        Dictionary<string, DPKMap> _dicDmkMaps = null;
        public void ProcessFolders(FileInfo inputPath)
        {
            try
            {
                //string inputPath = ConfigurationManager.AppSettings["Input"];
                DPKMappingCsv();
                ParseInput(inputPath.FullName);
                StringBuilder sb = new StringBuilder();
                string InprocessLocation = ConfigurationManager.AppSettings["OutboundInProcessLocation"].ToString();
                string ArchiveLocation = ConfigurationManager.AppSettings["OutboundArchiveLocation"].ToString();
                foreach (var item in _dicDmkMaps.Keys)
                {
                    string str = string.Empty;
                    var obj = _dicDmkMaps[item];
                    str = obj.strDpkValue + " ";
                    foreach (var ob in obj.lstProcessKeys)
                    {
                        str = str + ob.dpkKey + " " + ob.dpkPostion + " " + ob.Actualvalue + " ";
                    }
                    str = str + " " + obj.imagePath;
                    sb.Append(str.ToString());
                    sb.Append("\n");
                }
                //genereim

           string[] getImagePath = Directory.EnumerateFiles(InprocessLocation).
                    Where(fn => !Path.GetExtension(fn)
                    .Equals(".idx", StringComparison.OrdinalIgnoreCase)).ToArray();

                WriteOutputFile(Path.GetFileNameWithoutExtension(inputPath.FullName) + DateTime.Now.ToString("yyyyMMddhhmm") + ".imi", sb.ToString());
                if (File.Exists(Path.Combine(InprocessLocation, inputPath.Name)))
                    helpers.MoveFile(Path.Combine(InprocessLocation, inputPath.Name), Path.Combine(ArchiveLocation, inputPath.Name), true);
                string searchPattern = inputPath.Name.Replace(".idx", "") + "*";
                string[] fullFilePath = Directory.GetFiles(InprocessLocation, searchPattern);
                FileInfo fi = new FileInfo(fullFilePath[0]);
                if (File.Exists(Path.Combine(InprocessLocation, fi.Name)))
                    helpers.MoveFile(Path.Combine(InprocessLocation, fi.Name), Path.Combine(ArchiveLocation, fi.Name), true);


                string destinationpath = "";
                if (File.Exists(destinationpath))
                    helpers.CopyFile(destinationpath,
                        Path.Combine(ConfigurationManager.AppSettings["OutboundTrackingLocation"], "filename"), true);
                // Console.WriteLine();
            }
            catch (Exception ex)
            {

            }
        }

        public void WriteOutputFile(string filename, string content)
        {
            try
            {
                File.WriteAllText(Path.Combine(ConfigurationManager.AppSettings["OutboundOutputLocation"], filename), content);
                File.WriteAllText(Path.Combine(ConfigurationManager.AppSettings["OutboundTrackingLocation"], filename), content);
            }
            catch (Exception ex)
            {

            }

        }

        public void ParseInput(string path)
        {
            try
            {
                string[] inpArr = File.ReadAllLines(path);
                for (int i = 0; i < inpArr.Length; i++)
                {
                    string[] splitedline = inpArr[i].Split('|');

                    string dpkvalue = splitedline[28];
                    string imgPath = splitedline.Last();
                    _dicDmkMaps[dpkvalue].imagePath = imgPath;
                    var dictionary = _dicDmkMaps[dpkvalue];
                    for (int j = 0; j < dictionary.lstProcessKeys.Count; j++)
                    {
                        string key = dictionary.lstProcessKeys[j].dpkKey;
                        string position = dictionary.lstProcessKeys[j].dpkPostion;
                        string dmkReqFieldValues = splitedline[int.Parse(position)];
                        dictionary.lstProcessKeys[j].Actualvalue = dmkReqFieldValues;
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void DPKMappingCsv()
        {
            try
            {

                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = ConfigurationManager.AppSettings["DpkMapping"]; // full path to the config file
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                AppSettingsSection section = (AppSettingsSection)config.GetSection("appSettings");

                DPKMap objdmi = null;
                List<ProcessKey> lstprocKeys = null;
                ProcessKey processKey = null;
                _dicDmkMaps = new Dictionary<string, DPKMap>();
                foreach (var dpk in section.Settings.AllKeys)
                {
                    string dpkposVal = section.Settings[dpk].Value;
                    string[] linekeys = dpkposVal.Split(',');
                    lstprocKeys = new List<ProcessKey>();
                    foreach (var item in linekeys)
                    {
                        processKey = new ProcessKey();
                        string[] strsplit = item.Split('_');
                        processKey.dpkKey = strsplit[0];
                        processKey.dpkPostion = strsplit[1];
                        lstprocKeys.Add(processKey);
                    }
                    objdmi = new DPKMap();
                    objdmi.strDpkValue = dpk;
                    objdmi.lstProcessKeys = lstprocKeys;
                    _dicDmkMaps.Add(objdmi.strDpkValue, objdmi);
                }               
            }
            catch (Exception ex)
            {

            }
        }

    }
}
