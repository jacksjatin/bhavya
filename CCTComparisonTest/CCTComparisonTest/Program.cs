using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static CCTComparisonTest.csvhelper;

namespace CCTComparisonTest
{
    class Program
    {
        static string[] EdiArr;
        static List<csv> lstRec;
        static string inputFile = string.Empty;
        static void Main(string[] args)
        {
           
            string[] files = Directory.GetFiles(@"C:\Jatin\filedep2\IMI\CCT\Inp");
            lstRec = new List<CCTComparisonTest.csv>();
            for (int j = 0; j < files.Length; j++)
            {
                EdiArr = File.ReadAllLines(files[j]);                            
                List<claims> lst = new List<claims>();
                claims c = null;
                FileInfo fl = new FileInfo(files[j]);
                inputFile = fl.Name.Replace(".edi", "");
                for (int i = 0; i < EdiArr.Length; i++)
                {
                    char delimeter = EdiArr[i].Contains("*") ? '*' : EdiArr[i].Contains(">") ? '>' : '|';
                    if (EdiArr[i].StartsWith("NM1" + delimeter + "IL"))
                    {
                        c = new claims();
                        NML nml = new NML();
                        string[] nmlLine = EdiArr[i].Split(delimeter);
                        nml.nmlLineNumber = i;
                        nml.contract = nmlLine[9].ToString();
                        nml.firstName = nmlLine[4].ToString();
                        nml.nmlLineTxt = EdiArr[i];
                        c.nML = nml;
                    }
                    else if (EdiArr[i].StartsWith("NM1" + delimeter + "QC"))
                    {
                        NM1QC nM1QC = new NM1QC();
                        string[] nm1qcLine = EdiArr[i].Split(delimeter);
                        nM1QC.isnm1qc = true;
                        nM1QC.nm1qcFirstName = nm1qcLine[4];
                        nM1QC.nm1qcLineNumber = i;
                        nM1QC.nm1qcLineTxt = EdiArr[i];
                        c.nm1qc = nM1QC;

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

                
                File.WriteAllLines(Path.Combine(@"C:\Jatin\filedep2\IMI", fl.Name), EdiArr);
            }

            WriteCSV(lstRec, @"C:\Jatin\filedep2\IMI\output.csv");
        }

        public static void WriteCSV<T>(IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
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

            CCTComparisonTest2.Envelope env1 = Deserialize<CCTComparisonTest2.Envelope>(mResponse);
            foreach (var ob in env1.Body.NORMemberBPNSearchWSResponse.NORMemberBPNSearchWSResult.NORMemberInfoRecord)
            {
                FirstName = ob.FirstName;                
                if (item.nm1qc != null)
                {
                    if (FirstName == item.nm1qc.nm1qcFirstName)
                    {
                        MemberID = ob.MemberID;
                        count++;
                    }
                }
                else
                {
                    if (FirstName == item.nML.firstName)
                    {
                        MemberID = ob.MemberID;
                        count++;
                    }

                }
            }
            if (count == 1)
            {
                int counter = 0;
                bool isCont = true;
                string dcn = "0";
                //node = NewMethod(item, MemberID, contnum, ediAmount, ref count);
                while (isCont)
                {
                    Paging(item, MemberID, contnum,ref dcn, ediAmount, ref counter, ref isCont);
                }

            }
            else
            {
                return;
            }

        }

        private static void Paging(claims item, string MemberID, string contnum,ref string dcn, string ediAmount, ref int counter, ref bool isCont)
        {
            string InsSumResponse = GetInsSumRes(MemberID, contnum, ediAmount, dcn);           
            Envelope env2 = Deserialize<Envelope>(InsSumResponse);
            string tabledcn = string.Empty;
            foreach (var obj in env2.Body.NORInstitutionalSumMWWSResponse.NORInstitutionalSumMWWSResult.Claim_Table.CLAIM_TABLE)
            {
                string contNum = obj.O_CONT_NUM_R;
                tabledcn = obj.O_DCN_NO_R;
                string amount = obj.CHARGE;
                if (amount == ediAmount)
                {
                    counter++;
                }
            }

            if (counter == 1)
            {
                updatePCN2(tabledcn, item);
                isCont = false;
                return;
            }

            if (env2.Body.NORInstitutionalSumMWWSResponse.NORInstitutionalSumMWWSResult.StrLL_DCN_NO_R.StartsWith("16") ||
                env2.Body.NORInstitutionalSumMWWSResponse.NORInstitutionalSumMWWSResult.ERR_MSG.Contains("No Next")
                )
            {
                isCont = false;
                return;
            }
            dcn = env2.Body.NORInstitutionalSumMWWSResponse.NORInstitutionalSumMWWSResult.StrLL_DCN_NO_R;

        }

        public static T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }

        public static void updatePCN2(string newPcn, claims item)
        {
            csv c = new csv();
            char delimeter = item.cLM.clmLineTxt.Contains("*") ? '*' : item.cLM.clmLineTxt.Contains(">") ? '>' : '|';
            string[] oldcmlline = item.cLM.clmLineTxt.Split(delimeter);
            string oldpcn = oldcmlline[1];
            item.cLM.clmLineTxt = item.cLM.clmLineTxt.Replace(oldpcn, newPcn);
            EdiArr[item.cLM.clmLineNumber] = item.cLM.clmLineTxt;
            c.clmtype = inputFile.StartsWith("837I") ? "IN" : "PR";
            c.newpcn = newPcn;
            c.dcn = oldpcn;
            c.fileName = inputFile;

            lstRec.Add(c);



        }


        private static void updatePCN(string newPcn, claims item)
        {
            StreamWriter wtr = new StreamWriter(@"C:\Jatin\filedep2\IMI\CCT\837I-ND-20170728-tbc15-modif.txt");
            var e = File.ReadLines(@"C:\Jatin\filedep2\IMI\CCT\837I-ND-20170728-tbc15.txt").GetEnumerator();
            int lineno = item.cLM.clmLineNumber + 1;
            int counter = 0;
            string line = string.Empty;
            while (e.MoveNext())
            {
                counter++;
                if (counter == lineno)
                    line = replaceLogic(e.Current, newPcn);
                else
                    line = e.Current;
                wtr.WriteLine(line);
            }
            wtr.Close();
        }

        private static string replaceLogic(string current, string newPcn)
        {

            char delimeter = delimeter = current.Contains("*") ? '*' : current.Contains(">") ? '>' : '|';
            string[] oldcmlline = current.Split(delimeter);
            string oldpcn = oldcmlline[1];
            current = current.Replace(oldpcn, newPcn);
            return current;
        }

        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(@"C:\Jatin\filedep2\IMI\CCT\837I-ND-20170728-tbc15.txt");
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        private static string GetInsSumRes(string memberID, string contnum, string ediAmount,string dcn)
        {
            string insres = string.Empty;
            if (dcn == "0")
            {
                insres = File.ReadAllText(@"C:\Jatin\filedep2\IMI\CCT\SoapResponse2.txt");
            }
            else
            {
                insres = File.ReadAllText(@"C:\Jatin\filedep2\IMI\CCT\"+ dcn +".txt");
            }
            string strFiledata = RemoveNameSpaces(insres);
            strFiledata = strFiledata.Replace("xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "");
            return strFiledata;
        }

        private static string FormateContactNumber(string conNum)
        {
            if (conNum.StartsWith("R"))
            {
                return conNum;
            }
            else
            {
                string modifiedCNum = new String(conNum.Where(Char.IsDigit).ToArray());
                conNum = modifiedCNum;
            }
            return conNum;
        }

        private static string GetMemberRecords(claims item)
        {
            string cnm = FormateContactNumber(item.nML.contract);
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
        public NM1QC nm1qc;
    }

    public class csv
    {
        public string dcn { get; set; }
        public string newpcn { get; set; }
        public string clmtype { get; set; }
        public string fileName { get; set; }
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

    public class NM1QC
    {
        public int nm1qcLineNumber;
        public string nm1qcFirstName;
        public string nm1qcLineTxt;
        public bool isnm1qc;
    }
}
