/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace CCTComparisonTest2
{
    [XmlRoot(ElementName = "NORMemberInfoCommon")]
    public class NORMemberInfoCommon
    {
        [XmlElement(ElementName = "MessageNumber")]
        public string MessageNumber { get; set; }
        [XmlElement(ElementName = "MessageText")]
        public string MessageText { get; set; }
        [XmlElement(ElementName = "WarningMessage")]
        public string WarningMessage { get; set; }
        [XmlElement(ElementName = "DisplayContract")]
        public string DisplayContract { get; set; }
        [XmlElement(ElementName = "RecordCount")]
        public string RecordCount { get; set; }
    }

    [XmlRoot(ElementName = "NORMemberInfoRecord")]
    public class NORMemberInfoRecord
    {
        [XmlElement(ElementName = "FirstName")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "LastName")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "MiddleInit")]
        public string MiddleInit { get; set; }
        [XmlElement(ElementName = "LastFirstName")]
        public string LastFirstName { get; set; }
        [XmlElement(ElementName = "StatusCode")]
        public string StatusCode { get; set; }
        [XmlElement(ElementName = "StatusDescription")]
        public string StatusDescription { get; set; }
        [XmlElement(ElementName = "RelShipCode")]
        public string RelShipCode { get; set; }
        [XmlElement(ElementName = "RelShipDesc")]
        public string RelShipDesc { get; set; }
        [XmlElement(ElementName = "MemberID")]
        public string MemberID { get; set; }
        [XmlElement(ElementName = "BirthDateCYMD")]
        public string BirthDateCYMD { get; set; }
        [XmlElement(ElementName = "FromDateCYMD")]
        public string FromDateCYMD { get; set; }
        [XmlElement(ElementName = "ThruDateCYMD")]
        public string ThruDateCYMD { get; set; }
        [XmlElement(ElementName = "Age")]
        public string Age { get; set; }
        [XmlElement(ElementName = "SSN")]
        public string SSN { get; set; }
        [XmlElement(ElementName = "DiffAddrInd")]
        public string DiffAddrInd { get; set; }
        [XmlElement(ElementName = "DiffAddrDesc")]
        public string DiffAddrDesc { get; set; }
        [XmlElement(ElementName = "TitlePrefix")]
        public string TitlePrefix { get; set; }
        [XmlElement(ElementName = "SexCode")]
        public string SexCode { get; set; }
        [XmlElement(ElementName = "SexDescription")]
        public string SexDescription { get; set; }
    }

    [XmlRoot(ElementName = "NORMemberBPNSearchWSResult")]
    public class NORMemberBPNSearchWSResult
    {
        [XmlElement(ElementName = "NORMemberInfoCommon")]
        public NORMemberInfoCommon NORMemberInfoCommon { get; set; }
        [XmlElement(ElementName = "NORMemberInfoRecord")]
        public List<NORMemberInfoRecord> NORMemberInfoRecord { get; set; }
    }

    [XmlRoot(ElementName = "NORMemberBPNSearchWSResponse")]
    public class NORMemberBPNSearchWSResponse
    {
        [XmlElement(ElementName = "NORMemberBPNSearchWSResult")]
        public NORMemberBPNSearchWSResult NORMemberBPNSearchWSResult { get; set; }
    }

    [XmlRoot(ElementName = "Body")]
    public class Body
    {
        [XmlElement(ElementName = "NORMemberBPNSearchWSResponse")]
        public NORMemberBPNSearchWSResponse NORMemberBPNSearchWSResponse { get; set; }
    }

    [XmlRoot(ElementName = "Envelope")]
    public class Envelope
    {
        [XmlElement(ElementName = "Body")]
        public Body Body { get; set; }
    }

}
