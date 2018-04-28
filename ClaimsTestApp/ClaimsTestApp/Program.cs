using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelDataReader;

namespace ClaimsTestApp
{
    class Program
    {

        static void Main(string[] args)
        {
            string oldpcn = string.Empty;
            string newpcn = string.Empty;
            string editype = string.Empty;
            string edipath = string.Empty;
            string amount = string.Empty;
            string contract = string.Empty;
            string receiveddate = string.Empty;

            DataTable data = new DataTable();
            string unidentified = string.Empty;
            data = ProcessExcel(ConfigurationManager.AppSettings["ExcelPath"]);
            DataTable exceldata = new DataTable();
            exceldata = data;
            DataView view = exceldata.DefaultView;
            foreach (DataRow row in data.Rows)
            {
                if (row[0].ToString() == "Scenario")
                {
                    continue;
                }
                contract = row[3].ToString().Trim();
                receiveddate = row[4].ToString().Trim();
                editype = row[5].ToString().Trim();
                amount = row[8].ToString().Trim();
                oldpcn = row[10].ToString().Trim();
                newpcn = row[11].ToString().Trim();
                if (!string.IsNullOrEmpty(receiveddate))
                {
                    edipath = Extract(receiveddate, editype);
                }
                if (!string.IsNullOrEmpty(edipath))
                {
                    bool statuscheck = false;
                    edipath = @"C:\Jatin\filedep2\IMI\Batch1";
                    DirectoryInfo di = new DirectoryInfo(edipath);
                    var edifiles = di.GetFiles();
                    foreach (var file in edifiles)
                    {
                        FileStream fstream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                        StreamReader sreader = new StreamReader(fstream);
                        string editext = sreader.ReadToEnd();
                        if (editext.Contains(contract) && (editext.Contains(oldpcn + "*" + amount) || editext.Contains(oldpcn + ">" + amount) || editext.Contains(oldpcn + "|" + amount)))
                        {
                            bool secheck = false;
                            statuscheck = true;
                            string hdr = GetHeader(editext);
                            string subscriberInfo = GetSubscriberInfo(editext, contract, oldpcn, ref secheck);
                            string latestHdr = GetNewHeader(subscriberInfo.Split('\n')[0], editext);
                            subscriberInfo = UpdatePatientCNumber(subscriberInfo, oldpcn, newpcn);
                            string newsubscriberInfo = FormateSubscriberInfo(subscriberInfo, secheck);
                            if (newsubscriberInfo != "")
                            {
                                subscriberInfo = newsubscriberInfo;
                            }
                            string ft = Getfooter(editext, hdr + subscriberInfo);
                            string fText = string.Empty;
                            string updatedftr = updateGESegment(hdr, ft);
                            if (updatedftr != "")
                            {
                                // fText = hdr + latestHdr + subscriberInfo + updatedftr;
                                fText = UpdateSESegmentCount(hdr, subscriberInfo, latestHdr, updatedftr);

                                //fText = fText + "\n\n\n";
                                fText = Regex.Replace(fText, @"^\s*$", "", RegexOptions.Multiline);
                            }
                            else
                            {
                                fText = UpdateSESegmentCount(hdr, subscriberInfo, latestHdr, ft);
                            }
                            File.WriteAllText(@"C:\Jatin\filedep2\EDI\IN.txt", fText);
                          //  writeToText(fText, file.Name, editype, newpcn);
                        }
                        sreader.Close();
                        fstream.Close();
                        if (statuscheck == true)
                        {
                            break;
                        }
                    }
                    if (statuscheck == false)
                    {
                        unidentified = unidentified + contract + "\r\n";
                    }
                }
            }
            writeToText(unidentified, "unidentified.txt", string.Empty, string.Empty);
            Console.WriteLine("Search completed");

        }

        private static string UpdateSESegmentCount(string hdr, string subscriberInfo, string latestHdr, string updatedftr)
        {
            string fText;
            string originalValue = string.Empty;
            string updatemsg = hdr + latestHdr + subscriberInfo;
            char SegDelimeter = hdr[105];
            char ElemDelimeter = hdr[103];
            string count = (updatemsg.Split(SegDelimeter).Count() - 2).ToString();
            string[] arr = updatedftr.Split('\n');
            for (int i = 0; i < arr.Length; i++)
            {
                char delimeter = delimeter = arr[i].Contains("*") ? '*' : arr[i].Contains(">") ? '>' : '|';
                if (arr[i].Contains(string.Concat("SE" + delimeter)))
                {
                    string[] seArr = arr[i].Split(delimeter);
                    seArr[1] = count.ToString();
                    var lastValue = seArr.Last();
                    foreach (var item in seArr)
                    {
                        if (item.Equals(lastValue))
                        {
                            originalValue = originalValue + item;
                        }
                        else
                        {
                            originalValue = originalValue + item + delimeter.ToString();
                        }
                    }
                }
            }
            arr[0] = originalValue;
            string updatedSEVal = ConvertStringArrayToString(arr);
            updatedSEVal = Regex.Replace(updatedSEVal, @"^\s*$", "", RegexOptions.Multiline);
            fText = hdr + latestHdr + subscriberInfo + updatedSEVal;
            return fText;
        }

        private static string GetNewHeader(string sbrinfo, string editext)
        {
            try
            {

                string[] editxtArr = editext.Split('\n');
                List<string> list = new List<string>(editxtArr);
                int index = 0;
                int hlindx = 0;
                char delimeter;
                for (int i = 0; i < editxtArr.Length; i++)
                {
                    if (editxtArr[i].ToString() == sbrinfo)
                    {
                        index = i;
                        break;
                    }
                }

                for (int j = index; j >= 0; j--)
                {
                    delimeter = editxtArr[j].Contains("*") ? '*' : editxtArr[j].Contains(">") ? '>' : '|';

                    if (editxtArr[j].Contains(string.Concat("NM1" + delimeter + "85")))
                    {
                        List<string> sublist = list.GetRange(j, hlindx - j);
                        string output = string.Join("", sublist);
                        return output;
                    }
                    else if (editxtArr[j].Contains(string.Concat("HL" + delimeter)))
                    {
                        hlindx = j;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return "";
        }

        private static string FormateSubscriberInfo(string segmentinfo, bool SEcheck)
        {
            string finalOutput = string.Empty;
            try
            {
                string[] segmentedArr = segmentinfo.Split('\n');
                int count = 1;
                char delimeter;
                int HLIndex = 0;
                int SEIndex = 0;
                for (int i = 0; i < segmentedArr.Length; i++)
                {
                    delimeter = segmentedArr[i].Contains("*") ? '*' : segmentedArr[i].Contains(">") ? '>' : '|';

                    if (segmentedArr[i].Contains(string.Concat("HL" + delimeter)))
                    {
                        HLIndex = i;
                        string[] lineArr = segmentedArr[i].Split(delimeter);
                        count++;
                        lineArr[1] = count.ToString();
                        lineArr[2] = (count - 1).ToString();
                        var lastValue = lineArr.Last();
                        string originalValue = string.Empty;
                        foreach (var item in lineArr)
                        {
                            if (item.Equals(lastValue))
                            {
                                originalValue = originalValue + item;
                            }
                            else
                            {
                                originalValue = originalValue + item + delimeter.ToString();
                            }
                        }
                        segmentedArr[i] = originalValue;
                    }
                    //else if (segmentedArr[i].Contains(string.Concat("SE" + delimeter)))
                    //{
                    //    SEIndex = i;
                    //}
                }
                List<string> list = new List<string>(segmentedArr);
                if (SEcheck != true)
                {
                    list.RemoveRange(HLIndex, segmentedArr.Length - HLIndex);
                }
                finalOutput = ConvertStringArrayToString(list.ToArray());
            }
            catch (Exception ex)
            {

            }
            return finalOutput;
        }

        private static string updateGESegment(string hdr, string ftr)
        {

            string[] hdrArr = hdr.Split('\n');
            string[] ftrArr = ftr.Split('\n');
            string GEhdrSegment = string.Empty;
            string GEftrSegment = string.Empty;
            char delimeter;
            string[] gehdrLineArr = null;
            string[] geftrLineArr = null;
            string modifiedval = string.Empty;
            delimeter = hdrArr[1].Contains("*") ? '*' : hdrArr[1].Contains(">") ? '>' : '|';
            GEhdrSegment = hdrArr[1].Split(delimeter)[6].ToString();
            gehdrLineArr = hdrArr[1].Split(delimeter);
            GEftrSegment = ftrArr[1].Split(delimeter)[2].ToString();
            geftrLineArr = ftrArr[1].Split(delimeter);
            GEftrSegment = GEftrSegment.Replace("~\r", "");
            if (GEhdrSegment != GEftrSegment)
            {
                geftrLineArr[2] = gehdrLineArr[6];
                string last = geftrLineArr.Last();
                foreach (var item in geftrLineArr)
                {
                    if (item.Equals(last))
                    {
                        modifiedval = modifiedval + item;
                    }
                    else
                    {
                        modifiedval = modifiedval + item + delimeter;
                    }
                }

            }
            if (modifiedval != "")
            {
                ftrArr[1] = modifiedval.ToString() + "~\r";
                return ConvertStringArrayToString(ftrArr);
            }
            else
            {
                return "";
            }

        }

        private static string ConvertStringArrayToString(string[] array)
        {

            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append('\n');

            }
            return builder.ToString();
        }
        private static string Extract(string receiveddate, string type)
        {
            if (type == "IN")
            {
                type = "I";
            }
            else if (type == "PR")
            {
                type = "P";
            }
            return ConfigurationManager.AppSettings["EDISource"] + "837" + type + "-ND-" + "20170701-20180301";
        }
        private static string GetHeader(string message)
        {
            string header = string.Empty;
            char SegDelimeter = message[105];
            char ElemDelimeter = message[103];
            var segments = from seg in message.Split(SegDelimeter).Select(x => x.Trim())
                           where !string.IsNullOrEmpty(seg)
                           select new
                           {
                               segment = seg.Substring(0, seg.IndexOf(ElemDelimeter)),
                               data = seg
                           };
            int check = 0;
            foreach (var segment in segments)
            {
                if (segment.segment == "NM1" && segment.data.Contains("NM1" + ElemDelimeter + "85"))
                {
                    check = check + 1;
                    return header;
                }
                else
                {
                    header = header + segment.data.ToString() + SegDelimeter + "\r\n";
                }
            }
            return header;
        }
        private static string Getfooter(string msg, string updatemsg)
        {
            char SegDelimeter = msg[105];
            char ElemDelimeter = msg[103];
            string count = (updatemsg.Split(SegDelimeter).Count() - 2).ToString();
            string value = string.Empty;
            string footer = string.Empty;
            var segments = from seg in msg.Split(SegDelimeter).Select(x => x.Trim())
                           where !string.IsNullOrEmpty(seg)
                           select new
                           {
                               segment = seg.Substring(0, seg.IndexOf(ElemDelimeter)),
                               data = seg
                           };
            foreach (var segment in segments)
            {
                if (segment.segment == "ST")
                {
                    value = segment.data.Split(ElemDelimeter).Skip(2).First();
                    break;
                }
            }
            var foot = msg.Substring(msg.LastIndexOf("SE"), (msg.Length - msg.LastIndexOf("SE")));
            var fsegments = from seg in foot.Split(SegDelimeter).Select(x => x.Trim())
                            where !string.IsNullOrEmpty(seg)
                            select new
                            {
                                fsegment = seg.Substring(0, seg.IndexOf(ElemDelimeter)),
                                data = seg,
                                fdata = seg.Split(ElemDelimeter)
                            };
            foreach (var segment in fsegments)
            {
                if (segment.fsegment == "SE")
                {
                    footer = footer + segment.fdata[0] + ElemDelimeter + count + ElemDelimeter + value + SegDelimeter + "\r\n";
                }
                else if (segment.fsegment == "GE")
                {
                    footer = footer + segment.fdata[0] + ElemDelimeter + "1" + ElemDelimeter + segment.fdata[2] + SegDelimeter + "\r\n";
                }
                else if (segment.fsegment == "IEA")
                {
                    footer = footer + segment.fdata[0] + ElemDelimeter + "1" + ElemDelimeter + segment.fdata[2] + SegDelimeter + "\r\n";
                }
                else
                    footer = footer + segment.data;
            }
            return footer;
        }
        private static void writeToText(string text, string filename, string type, string npcn)
        {

           
            if (type == "IN")
            {
                filename = "InstOutput/" + npcn;
            }
            else if (type == "PR")
            {
                filename = "ProfOutput/" + npcn;
            }

            
           
        }
        private static string GetSubscriberInfo(string message, string contractnum, string opcn, ref bool secheck)
        {
            string ContractSegment = string.Empty;
            char SegDelimeter = message[105];
            char ElemDelimeter = message[103];
            string nm1 = "NM1" + ElemDelimeter + "IL";
            bool lxcheck = false;
            string NLMSegment = string.Empty;
            string totalText = string.Empty;
            string endText = string.Empty;
            var segments = from seg in message.Split(SegDelimeter).Select(x => x.Trim())
                           where !string.IsNullOrEmpty(seg)
                           select new
                           {
                               segment = seg.Substring(0, seg.IndexOf(ElemDelimeter)),
                               data = seg,

                           };
            foreach (var segment in segments)
            {
                if (!string.IsNullOrEmpty(ContractSegment))
                {
                    if (segment.data.Contains(nm1))
                    {
                        if (!ContractSegment.Contains(opcn))
                        {
                            totalText = totalText + ContractSegment;
                            ContractSegment = string.Empty;
                            if (segment.data.Contains(nm1) && segment.data.ToString().Contains(contractnum))
                            {
                                ContractSegment = ContractSegment + segment.data.ToString() + SegDelimeter + "\r\n";
                            }
                            continue;
                        }
                        else
                        {
                            if (lxcheck)
                            {
                                if (segment.segment == "SE" || segment.data.Contains(nm1))
                                {
                                    if (segment.segment == "SE")
                                    {
                                        secheck = true;
                                    }
                                    break;

                                }
                            }
                        }
                    }
                    if (segment.segment == "LX")
                    {
                        lxcheck = true;
                    }
                    if (segment.segment != "SE")
                    {
                        ContractSegment = ContractSegment + segment.data.ToString() + SegDelimeter + "\r\n";
                    }
                    else
                    {
                        secheck = true;
                        break;
                    }
                }
                else
                {
                    totalText = totalText + segment.data.ToString() + SegDelimeter + "\r\n";
                }
                if (segment.data.Contains(nm1) && segment.data.ToString().Contains(contractnum))
                {
                    ContractSegment = ContractSegment + segment.data.ToString() + SegDelimeter + "\r\n";
                    NLMSegment = segment.data.ToString();
                }
            }
            totalText = totalText.Remove(totalText.LastIndexOf("NM1"));
            string addSeg = totalText.Substring(totalText.LastIndexOf("HL"), (totalText.Length - totalText.LastIndexOf("HL")));
            return addSeg + ContractSegment;
        }
        private static string UpdatePatientCNumber(string subscriberInfo, string oldp, string newp)
        {
            return subscriberInfo.Replace(oldp, newp);
        }
        private static DataTable ProcessExcel(string filename)
        {
            DataTable table = null;
            if (!string.IsNullOrEmpty(filename))
            {
                FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                IExcelDataReader dr = ExcelReaderFactory.CreateOpenXmlReader(fs);
                DataSet ds = dr.AsDataSet();
                table = ds.Tables[0];
            }
            return table;
        }
    }
}





