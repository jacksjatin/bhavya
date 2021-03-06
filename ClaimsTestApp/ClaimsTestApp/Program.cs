﻿using System;
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
            string filePath = string.Empty;

            DataTable data = new DataTable();
            string unidentified = string.Empty;
            data = ProcessExcel(ConfigurationManager.AppSettings["ExcelPath"]);
            DataTable exceldata = new DataTable();
            exceldata = data;
            DataView view = exceldata.DefaultView;
            bool statuscheck = false;
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
                filePath = row[12].ToString().Trim();
                if (!string.IsNullOrEmpty(receiveddate))
                {
                    edipath = Extract(receiveddate, editype);
                }
                if (!string.IsNullOrEmpty(edipath))
                {
                    edipath = @"C:\Jatin\filedep2\IMI\Batch1";
                    //DirectoryInfo di = new DirectoryInfo(edipath);
                    //var edifiles = di.GetFiles();
                    //foreach (var file in edifiles)
                    //{
                        FileStream fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        StreamReader sreader = new StreamReader(fstream);
                        string editext = sreader.ReadToEnd();
                        if (editext.Contains(contract) && (editext.Contains(oldpcn + "*" + amount) || editext.Contains(oldpcn + ">" + amount) || editext.Contains(oldpcn + "|" + amount)))
                        {
                            bool secheck = false;
                            statuscheck = true;
                            string hdr = GetHeader(editext);
                            string subscriberInfo = GetSubscriberInfo(editext, contract, oldpcn, amount, ref secheck);
                            string latestHdr = GetNewHeader(subscriberInfo.Split('\n')[0], editext, oldpcn, amount);
                            subscriberInfo = UpdatePatientCNumber(subscriberInfo, oldpcn, newpcn);
                            int clmcount = Regex.Matches(subscriberInfo, "CLM").Count;
                            if (clmcount > 1)
                            {
                                string str = processMultiCLMS(subscriberInfo, newpcn, amount);
                                subscriberInfo = str;
                            }
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
                        else
                        {
                            sreader.Close();
                            fstream.Close();
                            unidentified = contract + "\r\n";
                        }
                        if (statuscheck == true)
                        {
                            break;
                        }
                   // }
                    Console.WriteLine("Search completed");
                    //if (statuscheck == false)
                    //{
                    //    unidentified = unidentified + contract + "\r\n";
                    //}
                }
                if (statuscheck == false)
                {
                    writeToText(unidentified, "unidentified.txt", string.Empty, string.Empty);
                }
            }

        }

        private static string processMultiCLMS(string subscriberInfo, string oldpcn, string amount)
        {
            string[] arr = subscriberInfo.Split('\n');
            int startIndex = 0;

            List<int> N4index = new List<int>();
            List<int> DTPIndex = new List<int>();

            int temp = 0;
            int endIndex = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                char delimeter = delimeter = arr[i].Contains("*") ? '*' : arr[i].Contains(">") ? '>' : '|';
                if (arr[i].Contains(string.Concat("CLM" + delimeter)) && arr[i].Contains(oldpcn) && arr[i].Contains(amount))
                {
                    startIndex = i;
                }

                if (arr[i].Contains("N4" + delimeter))
                {
                    N4index.Add(i);
                }
                if (arr[i].Contains("DTP" + delimeter))
                {
                    DTPIndex.Add(i);
                }
                if (startIndex > 0)
                {
                    if (arr[i].Contains("SV"))
                    {
                        endIndex = i;
                        temp = startIndex;
                        startIndex = 0;
                    }
                }
            }
            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < arr.Length; j++)
            {
                if (j <= N4index[0])
                {
                    sb.Append(arr[j]);
                }

                if (j >= temp && j <= endIndex)
                {
                    sb.Append(arr[j]);
                }

                if (j >= DTPIndex[DTPIndex.Count - 1] && j <= arr.Length - 1)
                {
                    sb.Append(arr[j]);
                }
            }
            return sb.ToString();
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
                char delimeter = arr[i].Contains("*") ? '*' : arr[i].Contains(">") ? '>' : '|';
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

        private static string GetNewHeader(string sbrinfo, string editext, string opcn, string amount)
        {
            try
            {
                string[] editxtArr = editext.Split('\n');
                bool check = false;
                List<string> list = new List<string>(editxtArr);
                int index = 0;
                int hlindx = 0;
                string[] Hlarr;
                char tempdelimeter = sbrinfo.Contains("*") ? '*' : sbrinfo.Contains(">") ? '>' : '|';
                string opcnAmount = opcn + tempdelimeter + amount;
                Hlarr = sbrinfo.Split(tempdelimeter);
                for (int i = 0; i < editxtArr.Length; i++)
                {
                    if (editxtArr[i].ToString() == sbrinfo)
                    {
                        index = i;
                        //break;
                    }
                    if (editxtArr[i].ToString().Contains(opcnAmount))
                    {
                        break;
                    }

                }

                for (int j = index; j >= 0; j--)
                {
                    // delimeter = editxtArr[j].Contains("*") ? '*' : editxtArr[j].Contains(">") ? '>' : '|';  
                    if (!check)
                    {
                        string[] checkHl = editxtArr[j].Split(tempdelimeter);
                        if (checkHl[1] == checkHl[2])
                        {
                            hlindx = j;
                            check = true;
                            continue;
                        }
                    }
                    if(Hlarr[2] == "")
                    {
                        for (int m = j - 1 ; m >= 0; m--)
                        {
                            if (editxtArr[m].StartsWith(string.Concat("HL" + tempdelimeter)))
                            {
                                List<string> sublist = list.GetRange(m + 1, j - m - 1);
                                string output = string.Join("", sublist);
                                return output;
                            }
                        }
                    }
                    if (editxtArr[j].StartsWith(string.Concat(Hlarr[0] + tempdelimeter + Hlarr[2] + tempdelimeter)))
                    {
                        for (int m = j + 1; m <= hlindx; m++)
                        {
                            if (editxtArr[m].StartsWith(string.Concat("HL" + tempdelimeter)))
                            {
                                List<string> sublist = list.GetRange(j + 1, m - j - 1);
                                string output = string.Join("", sublist);
                                return output;
                            }
                        }

                    }
                    else if (editxtArr[j].StartsWith(string.Concat("HL" + tempdelimeter)))
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

            string isahdrSegment = string.Empty;
            isahdrSegment = hdrArr[0].Split(delimeter)[13].ToString();
            string[] isaftrarr = ftrArr[2].Split(delimeter);
            string isaval = isaftrarr[2].Replace("~\r", "");
            string ftrline2 = string.Empty;
            if (isahdrSegment != isaval)
            {
                isaftrarr[2] = isahdrSegment;
                string lst = isaftrarr.Last();
                foreach (var item in isaftrarr)
                {
                    if (item.Equals(lst))
                    {
                        ftrline2 = ftrline2 + item;
                    }
                    else
                    {
                        ftrline2 = ftrline2 + item + delimeter;
                    }
                }
            }

            if (modifiedval != "")
            {
                ftrArr[1] = modifiedval.ToString() + "~\r";
            }
            if (ftrline2 != "")
            {
                ftrArr[2] = ftrline2.ToString() + "~\r";
            }
            //else
            //{
            //    return "";
            //}
            return ConvertStringArrayToString(ftrArr);
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
                if (segment.segment == "HL" && segment.data.Contains("HL" + ElemDelimeter + "1"))
                {
                    check = check + 1;
                    header = header + segment.data.ToString() + SegDelimeter + "\r\n";
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
        private static string GetSubscriberInfo(string message, string contractnum, string opcn, string amount, ref bool secheck)
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

            string opcnAmount = opcn + ElemDelimeter + amount;
            foreach (var segment in segments)
            {
                if (!string.IsNullOrEmpty(ContractSegment))
                {
                    if (segment.data.Contains(nm1))
                    {
                        if (!ContractSegment.Contains(opcn))
                        {
                            // totalText = totalText + ContractSegment;
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
                        if (!(segment.data.Contains(nm1) && segment.data.ToString().Contains(contractnum)))
                        {
                            ContractSegment = ContractSegment + segment.data.ToString() + SegDelimeter + "\r\n";
                        }
                    }
                    else
                    {
                        if (!ContractSegment.Contains(opcnAmount))
                        {
                            ContractSegment = "";
                            continue;
                        }
                        else
                        {
                            secheck = true;
                            break;
                        }
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





