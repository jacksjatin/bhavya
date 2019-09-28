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
        public List<string> imagePath;
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
                //DPKMappingCsv();
                //ParseInput(inputPath.FullName);

                List<ProcessKey> mappingKeyValueList = MappingList();
                updateMappingList(inputPath.FullName, ref mappingKeyValueList);
               
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
                    foreach (var img in obj.imagePath)
                    {
                        str = str + " " + obj.imagePath;
                    }
                    sb.Append(str.ToString());
                    sb.Append("\n");
                }
                //genereim

                bool isFolder = false;

                string idxcontent = File.ReadAllText(inputPath.FullName);
                string[] allSegm = idxcontent.Split('|');
                string imgPath = allSegm.Last();
                FileInfo finf = new FileInfo(imgPath);
                string folderName = finf.Directory.Name;

                if (Directory.Exists(Path.Combine(InprocessLocation, folderName)))
                {
                    string[] getImagePath = Directory.EnumerateFiles(Path.Combine(InprocessLocation, folderName.Replace(".idx", ""))).
                    Where(fn => !Path.GetExtension(fn)
                    .Equals(".idx", StringComparison.OrdinalIgnoreCase)).ToArray();
                    isFolder = true;
                    string mergedImgPath = findImagePath(getImagePath, folderName.Replace(".idx", ""));
                }
                else if (File.Exists(Path.Combine(InprocessLocation, folderName)))
                {

                    DirectoryInfo dir = new DirectoryInfo(InprocessLocation);
                    FileInfo[] files = dir.GetFiles(folderName.Replace(".idx", "") + "*", SearchOption.TopDirectoryOnly)
                    .Where(f => f.Extension != ".idx").ToArray<FileInfo>();
                    //string[] getImagePath = Directory.EnumerateFiles(InprocessLocation).
                    //Where(fn => !Path.GetExtension(fn)
                    //.Equals(".idx", StringComparison.OrdinalIgnoreCase)).ToArray();
                    string s = files[0].FullName.ToString();
                }

                WriteOutputFile(Path.GetFileNameWithoutExtension(inputPath.FullName) + DateTime.Now.ToString("yyyyMMddhhmm") + ".imi", sb.ToString());
                if (isFolder)
                {
                    if (File.Exists(Path.Combine(InprocessLocation, inputPath.Name)))
                        helpers.MoveFile(Path.Combine(InprocessLocation, inputPath.Name), Path.Combine(ArchiveLocation, inputPath.Name), true);
                    if (Directory.Exists(Path.Combine(InprocessLocation, folderName.Replace(".idx", ""))))
                    {
                        if (Directory.Exists(Path.Combine(ArchiveLocation, folderName.Replace(".idx", ""))))
                        {
                            Directory.Delete(Path.Combine(ArchiveLocation, folderName.Replace(".idx", "")), true);
                        }
                        Directory.Move(Path.Combine(InprocessLocation, folderName.Replace(".idx", "")), Path.Combine(ArchiveLocation, folderName.Replace(".idx", "")));
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(InprocessLocation, inputPath.Name)))
                        helpers.MoveFile(Path.Combine(InprocessLocation, inputPath.Name), Path.Combine(ArchiveLocation, inputPath.Name), true);
                    string searchPattern = inputPath.Name.Replace(".idx", "") + "*";
                    string[] fullFilePath = Directory.GetFiles(InprocessLocation, searchPattern);
                    FileInfo fi = new FileInfo(fullFilePath[0]);
                    if (File.Exists(Path.Combine(InprocessLocation, fi.Name)))
                        helpers.MoveFile(Path.Combine(InprocessLocation, fi.Name), Path.Combine(ArchiveLocation, fi.Name), true);
                }
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


        public string findImagePath(string[] sArry, string imgName)
        {
            string foundImgPath = string.Empty;
            foreach (var item in sArry)
            {
                FileInfo fl = new FileInfo(item);
                if (fl.Name.Contains(imgName))
                {
                    foundImgPath = fl.FullName;
                    return foundImgPath;
                }
            }
            return foundImgPath;
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

                    string dpkvalue = splitedline[27];
                    string imgPath = splitedline.Last();
                    if (_dicDmkMaps[dpkvalue].imagePath == null)
                    {
                        _dicDmkMaps[dpkvalue].imagePath = new List<string>();
                        _dicDmkMaps[dpkvalue].imagePath.Add(imgPath);
                    }
                    else
                    {
                        _dicDmkMaps[dpkvalue].imagePath.Add(imgPath);
                    }
                    var dictionary = _dicDmkMaps[dpkvalue];
                    for (int j = 0; j < dictionary.lstProcessKeys.Count; j++)
                    {

                        string key = dictionary.lstProcessKeys[j].dpkKey;
                        string position = dictionary.lstProcessKeys[j].dpkPostion;

                        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(position))
                        {
                            dictionary.lstProcessKeys[j].Actualvalue = "";
                        }
                        else
                        {
                            string dmkReqFieldValues = splitedline[int.Parse(position)];
                            dictionary.lstProcessKeys[j].Actualvalue = dmkReqFieldValues;
                        }
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
                        if (string.IsNullOrEmpty(item))
                        {
                            processKey.dpkKey = "";
                            processKey.dpkPostion = "";
                            lstprocKeys.Add(processKey);
                        }
                        else
                        {
                            string[] strsplit = item.Split('_');
                            processKey.dpkKey = strsplit[0];
                            processKey.dpkPostion = strsplit[1];
                            lstprocKeys.Add(processKey);
                        }
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

        public void updateMappingList(string path, ref List<ProcessKey> lstProcessKeys)
        {
            string[] splitedlines = File.ReadAllLines(path).First().Split('|');

            for (int i = 19; i < splitedlines.Length; i++)
            {
                var obj = lstProcessKeys.Find(x => x.dpkPostion == i.ToString());
                obj.Actualvalue = splitedlines[i];
            }
        }

        public List<ProcessKey> MappingList()
        {
            string[] maplist = ConfigurationManager.AppSettings["MappingKeys"].ToString().Split(',');
            List<ProcessKey> lstProcessKeys = new List<ProcessKey>();

            foreach (var item in maplist)
            {
                string[] keyValue = item.Split('_');
                lstProcessKeys.Add(new ProcessKey
                {
                    dpkKey = keyValue[0],
                    dpkPostion = keyValue[1]
                });
            }
            return lstProcessKeys;
        }
    }
}
