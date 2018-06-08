using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CCTComparisonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] EdiArr = File.ReadAllLines(@"C:\Jatin\filedep2\IMI\CCT\837I-ND-20170728-tbc15.txt");
            List<claims> lst = new List<claims>();
            claims c = null;
            for (int i = 0; i < EdiArr.Length; i++)
            {
                char delimeter = delimeter = EdiArr[i].Contains("*") ? '*' : EdiArr[i].Contains(">") ? '>' : '|';
                if (EdiArr[i].StartsWith("NM1" + delimeter + "IL"))
                {
                    c = new claims();
                    NML nml = new NML();
                    string[] nmlLine = EdiArr[i].Split(delimeter);
                    nml.nmlLineNumber = i;
                    nml.contract = nmlLine[9].ToString();
                    nml.firstName = nmlLine[3].ToString();
                    nml.nmlLineTxt = EdiArr[i];
                    c.nML = nml;
                }
                else if (EdiArr[i].StartsWith("CLM" + delimeter))
                {
                    CLM clm = new CLM();
                    string[] clmLine = EdiArr[i].Split(delimeter);
                    clm.clmLineNumber = i;
                    clm.oldPcn = clmLine[1];
                    clm.amount = clmLine[2];
                    clm.clmLineTxt = EdiArr[i];
                    c.cLM = clm;
                    lst.Add(c);
                }
                else
                {
                    continue;
                }

            }


            foreach (var item in lst)
            {
                processMemberResponse(item);
            }
        }

        public static void processMemberResponse(claims item)
        {

            string mResponse = GetMemberRecords(item);
            string FirstName = string.Empty;
            string MemberID = string.Empty;
            string contnum = item.nML.contract;
            string ediAmount = item.cLM.amount;
            string node = string.Empty;
            int count = 0;
            XElement xElem = XElement.Load(new StringReader(mResponse));
            IEnumerable<XElement> xElement = null;

            //Get MemberID
            node = "NORMemberInfoRecord";
            xElement = from ele in xElem.Descendants(node)
                       select ele;
            if (xElement.Count() > 0)
            {
                foreach (XElement n in xElement)
                {
                    FirstName = n.Element("FirstName").Value;
                    MemberID = n.Element("MemberID").Value;
                    if (FirstName == item.nML.firstName)
                    {
                        string rec = n.ToString();
                        count++;
                    }
                }
            }

            if (count == 1)
            {
                string InsSumResponse = GetInsSumRes(MemberID, contnum, ediAmount);
                XElement xinsElem = XElement.Load(new StringReader(InsSumResponse));
                IEnumerable<XElement> xinsElement = null;

                //Get DCN
                node = "CLAIM_TABLE";
                xinsElement = from ele in xinsElem.Descendants(node)
                           select ele;
                if (xinsElement.Count() > 0)
                {
                    foreach (XElement n in xinsElement)
                    {
                        string contNum = n.Element("O_CONT_NUM_R").Value;
                        string dcn = n.Element("O_DCN_NO_R").Value;
                        string amount = n.Element("CHARGE").Value;
                        if (amount == ediAmount)
                        {
                            string rec = n.ToString();
                            count++;
                        }
                    }
                }
            }
            else
            {
                return;
            }

        }

        private static string GetInsSumRes(string memberID,string contnum,string ediAmount)
        {
            string insres = File.ReadAllText(@"C:\Jatin\filedep2\IMI\CCT\SoapResponse2.txt");
            string strFiledata = RemoveNameSpaces(insres);
            strFiledata = strFiledata.Replace("xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "");
            return strFiledata;
        }

        private static string GetMemberRecords(claims item)
        {
            string memberinfoRes = File.ReadAllText(@"C:\Jatin\filedep2\IMI\CCT\SoapResponse1.txt");
            string strFiledata = RemoveNameSpaces(memberinfoRes);            
            strFiledata = strFiledata.Replace("xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "");
            return strFiledata;
        }

        public static string RemoveNameSpaces(string strFiledata)
        {
            try
            {
                XDocument doc = XDocument.Parse(strFiledata, LoadOptions.PreserveWhitespace);
                doc.Descendants().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
                strFiledata = doc.ToString().Replace("xmlns=\"http://da.noridian.com/NORMemberInfoWS/NORMemberInfo\"", "");
                strFiledata = strFiledata.Replace("xmlns=\"http://da.noridian.com/NORClaimsDataWS/Inst\"", "");
                strFiledata = strFiledata.Replace("xmlns=\"http://tempuri.org/XMLSchema.xsd\"", "");
            }
            catch (Exception ex)
            {

            }
            return strFiledata;
        }
    }

    public class claims
    {
        public NML nML;
        public CLM cLM;
    }

    public class NML
    {
        public int nmlLineNumber;
        public string contract;
        public string firstName;
        public string nmlLineTxt;
    }

    public class CLM
    {
        public int clmLineNumber;
        public string amount;
        public string oldPcn;
        public string clmLineTxt;
    }
}
