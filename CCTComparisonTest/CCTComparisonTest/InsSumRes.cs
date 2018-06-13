/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace CCTComparisonTest
{
    [XmlRoot(ElementName = "CLAIM_TABLE")]
    public class CLAIM_TABLE
    {
        [XmlElement(ElementName = "O_CONT_PRE")]
        public string O_CONT_PRE { get; set; }
        [XmlElement(ElementName = "O_CONT_NUM_R")]
        public string O_CONT_NUM_R { get; set; }
        [XmlElement(ElementName = "O_DCN_NO_R")]
        public string O_DCN_NO_R { get; set; }
        [XmlElement(ElementName = "O_DCN_SUF")]
        public string O_DCN_SUF { get; set; }
        [XmlElement(ElementName = "O_OUTST_OR_HIST")]
        public string O_OUTST_OR_HIST { get; set; }
        [XmlElement(ElementName = "O_MEMBER")]
        public string O_MEMBER { get; set; }
        [XmlElement(ElementName = "O_ADMIT_CYMD_9")]
        public string O_ADMIT_CYMD_9 { get; set; }
        [XmlElement(ElementName = "FROM_CYMD_9")]
        public string FROM_CYMD_9 { get; set; }
        [XmlElement(ElementName = "THRU_CYMD_9")]
        public string THRU_CYMD_9 { get; set; }
        [XmlElement(ElementName = "PROV_NUM")]
        public string PROV_NUM { get; set; }
        [XmlElement(ElementName = "PROV_NAME")]
        public string PROV_NAME { get; set; }
        [XmlElement(ElementName = "O_EIN_SSN")]
        public string O_EIN_SSN { get; set; }
        [XmlElement(ElementName = "FORMATTED_DIAG_ICD9")]
        public string FORMATTED_DIAG_ICD9 { get; set; }
        [XmlElement(ElementName = "DIAG_DESC")]
        public string DIAG_DESC { get; set; }
        [XmlElement(ElementName = "ADJUST_IND")]
        public string ADJUST_IND { get; set; }
        [XmlElement(ElementName = "CHARGE")]
        public string CHARGE { get; set; }
        [XmlElement(ElementName = "PAID")]
        public string PAID { get; set; }
        [XmlElement(ElementName = "PAID_AMT_FIELD")]
        public string PAID_AMT_FIELD { get; set; }
        [XmlElement(ElementName = "SUB_LIABLE")]
        public string SUB_LIABLE { get; set; }
        [XmlElement(ElementName = "CAPITATED_MSG")]
        public string CAPITATED_MSG { get; set; }
        [XmlElement(ElementName = "RESERVE_AMT")]
        public string RESERVE_AMT { get; set; }
        [XmlElement(ElementName = "SERV_BEN_TOT")]
        public string SERV_BEN_TOT { get; set; }
        [XmlElement(ElementName = "CNA_TOT")]
        public string CNA_TOT { get; set; }
        [XmlElement(ElementName = "PAID_OON_LEVEL")]
        public string PAID_OON_LEVEL { get; set; }
        [XmlElement(ElementName = "SCCF_NUM")]
        public string SCCF_NUM { get; set; }
    }

    [XmlRoot(ElementName = "Claim_Table")]
    public class Claim_Table
    {
        [XmlElement(ElementName = "CLAIM_TABLE")]
        public List<CLAIM_TABLE> CLAIM_TABLE { get; set; }
    }

    [XmlRoot(ElementName = "NORInstitutionalSumMWWSResult")]
    public class NORInstitutionalSumMWWSResult
    {
        [XmlElement(ElementName = "ObjCommon")]
        public string ObjCommon { get; set; }
        [XmlElement(ElementName = "Claim_Table")]
        public Claim_Table Claim_Table { get; set; }
        [XmlElement(ElementName = "StrFL_DCN_NO_R")]
        public string StrFL_DCN_NO_R { get; set; }
        [XmlElement(ElementName = "IntFL_DCN_SUFF")]
        public string IntFL_DCN_SUFF { get; set; }
        [XmlElement(ElementName = "StrLL_DCN_NO_R")]
        public string StrLL_DCN_NO_R { get; set; }
        [XmlElement(ElementName = "IntLL_DCN_SUF")]
        public string IntLL_DCN_SUF { get; set; }
        [XmlElement(ElementName = "Int_LAST_ENTRY")]
        public string Int_LAST_ENTRY { get; set; }
        [XmlElement(ElementName = "FL_OUTST_OR_HIST")]
        public string FL_OUTST_OR_HIST { get; set; }
        [XmlElement(ElementName = "LL_OUTST_OR_HIST")]
        public string LL_OUTST_OR_HIST { get; set; }
        [XmlElement(ElementName = "ERR_MSG")]
        public string ERR_MSG { get; set; }
        [XmlElement(ElementName = "ERR_NUM")]
        public string ERR_NUM { get; set; }
    }

    [XmlRoot(ElementName = "NORInstitutionalSumMWWSResponse")]
    public class NORInstitutionalSumMWWSResponse
    {
        [XmlElement(ElementName = "NORInstitutionalSumMWWSResult")]
        public NORInstitutionalSumMWWSResult NORInstitutionalSumMWWSResult { get; set; }
    }

    [XmlRoot(ElementName = "Body")]
    public class Body
    {
        [XmlElement(ElementName = "NORInstitutionalSumMWWSResponse")]
        public NORInstitutionalSumMWWSResponse NORInstitutionalSumMWWSResponse { get; set; }
    }

    [XmlRoot(ElementName = "Envelope")]
    public class Envelope
    {
        [XmlElement(ElementName = "Body")]
        public Body Body { get; set; }
    }

}
