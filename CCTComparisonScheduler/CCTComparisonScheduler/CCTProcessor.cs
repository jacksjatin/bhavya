using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CCTComparisonScheduler
{
    public class CCTProcessor
    {
        public string[] EdiArr;       
        public string inputFile = string.Empty;
        public List<csv> lstcsv = new List<csv>();
        private Helpers helpers;
        public bool isUpdated = false;
        public int totalClaimsCur = 0;
        public int totalClaimsUpdatedCur = 0;
        public int totalClaimsMatchedCur = 0;
        public int totalMembersMatchedCur = 0;
        public int totalUnknownContractsCur = 0;
        public int totalUnknownClaimCur = 0;
        public CCTProcessor()
        {
            helpers = new Helpers();
        }
        public void CompareContract(FileInfo fi,ref List<csv> lstRec,ref int totalClaims, ref int totalClaimsUpdated, ref int totalClaimsMatched, ref 
            int totalMembersMatched, ref int totalUnknownContracts, ref int totalUnknownClaim
            )
        {

            EdiArr = File.ReadAllLines(fi.FullName);
            List<claims> lst = new List<claims>();
            claims c = null;            
            inputFile = fi.Name.Replace(".edi", "");
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
                    totalClaimsCur++;
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

            lstRec.AddRange(lstcsv);
            totalClaims = totalClaims + totalClaimsCur;
            totalClaimsUpdated = totalClaimsUpdated + totalClaimsUpdatedCur;
            string outputLoc = ConfigurationManager.AppSettings["OutputLocation"].ToString();
            string archiveLoc = ConfigurationManager.AppSettings["ArchiveLocation"].ToString();
            string InprocessLocation = ConfigurationManager.AppSettings["InProcessLocation"].ToString();
            string suppressedLocation = ConfigurationManager.AppSettings["SuppressedLocation"].ToString();
            if (isUpdated)
            {
                File.WriteAllLines(Path.Combine(outputLoc, inputFile), EdiArr);
                if (File.Exists(Path.Combine(InprocessLocation, fi.Name)))
                    helpers.MoveFile(Path.Combine(InprocessLocation, fi.Name), Path.Combine(archiveLoc, fi.Name), true);
            }
            else
            {
                if (File.Exists(Path.Combine(InprocessLocation, fi.Name)))
                    helpers.MoveFile(Path.Combine(InprocessLocation, fi.Name), Path.Combine(suppressedLocation, fi.Name), true);
            }
            
        }

        public void processMemberResponse(claims item)
        {

            string mResponse = GetMemberRecords(item);
            string FirstName = string.Empty;
            string MemberID = string.Empty;
            string contnum = item.nML.contract;
            string ediAmount = item.cLM.amount;
            string node = string.Empty;
            int count = 0;

            soapClasses.Envelope env1 = Deserialize<soapClasses.Envelope>(mResponse);
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
                    Paging(item, MemberID, contnum, ref dcn, ediAmount, ref counter, ref isCont);
                }
            }
            else
            {
                return;
            }

        }


        private void Paging(claims item, string MemberID, string contnum, ref string dcn, string ediAmount, ref int counter, ref bool isCont)
        {
            string InsSumResponse = GetInsSumRes(MemberID, contnum, ediAmount, dcn);
            soapClasses2.Envelope env2 = Deserialize<soapClasses2.Envelope>(InsSumResponse);
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
                updatePCN(tabledcn, item);
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

        public void updatePCN(string newPcn, claims item)
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
            isUpdated = true;
            lstcsv.Add(c);



        }


        private string GetInsSumRes(string memberID, string contnum, string ediAmount, string dcn)
        {
            string insres = string.Empty;
            if (dcn == "0")
            {
                insres = File.ReadAllText(@"C:\Jatin\filedep2\IMI\CCT\SoapResponse2.txt");
            }
            else
            {
                insres = File.ReadAllText(@"C:\Jatin\filedep2\IMI\CCT\" + dcn + ".txt");
            }
            string strFiledata = RemoveNameSpaces(insres);
            strFiledata = strFiledata.Replace("xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "");
            return strFiledata;
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

        private string GetMemberRecords(claims item)
        {
            string cnm = FormateContactNumber(item.nML.contract);
            string memberinfoRes = File.ReadAllText(@"C:\Jatin\filedep2\IMI\CCT\SoapResponse1.txt");
            string strFiledata = RemoveNameSpaces(memberinfoRes);
            strFiledata = strFiledata.Replace("xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "");
            return strFiledata;
        }

        public string RemoveNameSpaces(string strFiledata)
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

        private string FormateContactNumber(string conNum)
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
        public int nmlLineNumber ;
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
