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
    public class DMKMap
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
        Dictionary<string, DMKMap> _dicDmkMaps = null;      
        public void ProcessFolders(string inputPath)
        {
            try
            {
                //string inputPath = ConfigurationManager.AppSettings["Input"];

                DPKMappingCsv();
                ParseInput(inputPath);

                StringBuilder sb = new StringBuilder();
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
                Console.WriteLine(sb.ToString());
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
                string fileName = ConfigurationManager.AppSettings["DPKMapping"];
                using (var stream = File.OpenRead(fileName))
                using (var reader = new StreamReader(stream))
                {
                    var data = CsvParser.ParseHeadAndTail(reader, ',', '"');
                    var header = data.Item1;
                    var lines = data.Item2;
                    DMKMap objdmi = null;
                    List<ProcessKey> lstprocKeys = null;
                    ProcessKey processKey = null;
                    _dicDmkMaps = new Dictionary<string, DMKMap>();
                    foreach (var line in lines)
                    {
                        string[] linekeys = line[1].Split(',');
                        lstprocKeys = new List<ProcessKey>();

                        foreach (var item in linekeys)
                        {
                            processKey = new ProcessKey();
                            string[] strsplit = item.Split('_');
                            processKey.dpkKey = strsplit[0];
                            processKey.dpkPostion = strsplit[1];
                            lstprocKeys.Add(processKey);
                        }
                        objdmi = new DMKMap();
                        objdmi.strDpkValue = line[0];
                        objdmi.lstProcessKeys = lstprocKeys;
                        _dicDmkMaps.Add(objdmi.strDpkValue, objdmi);

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
