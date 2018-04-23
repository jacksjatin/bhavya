using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIAcknowledgementValidator
{
    public class ValidateAckFile
    {
        private HelperClasses.Helpers helpers;
        public void validateAcknowledgememt(FileInfo fi)
        {

            try
            {
                helpers = new HelperClasses.Helpers();
                string ackData = string.Empty;

                using (StreamReader sr = new StreamReader(fi.FullName))
                {
                    ackData = sr.ReadToEnd();
                }
                bool Success = false;
                string SourceLocation = ConfigurationManager.AppSettings["SourceLocation"].ToString();
                string TrackingLocation = ConfigurationManager.AppSettings["TrackingLocation"].ToString();
                string ArchiveLocation = ConfigurationManager.AppSettings["ArchiveLocation"].ToString();

                string ReconcileLocation = ConfigurationManager.AppSettings["ReconcileLocation"].ToString();
                string[] SuccessCode = ConfigurationManager.AppSettings["SuccessCodes"].Split(',');
                for (int i = 0; i < SuccessCode.Length; i++)
                {
                    if (ackData.Contains(SuccessCode[i]))
                    {
                        Success = true;
                    }
                }
                string imifilename = fi.Name.Replace(".ack", ".imi");
                if (!Success)
                {
                    if (File.Exists(Path.Combine(TrackingLocation, imifilename)))
                    {
                        helpers.MoveFile(Path.Combine(TrackingLocation, imifilename), Path.Combine(ReconcileLocation, imifilename), true);
                        if (File.Exists(Path.Combine(SourceLocation, fi.Name)))
                            helpers.MoveFile(Path.Combine(SourceLocation, fi.Name), Path.Combine(ReconcileLocation, fi.Name), true);
                    }
                }
                else
                {
                    
                    if (File.Exists(Path.Combine(TrackingLocation, imifilename)))
                    {
                        helpers.MoveFile(Path.Combine(TrackingLocation, imifilename), Path.Combine(ArchiveLocation, imifilename), true);
                        if (File.Exists(Path.Combine(SourceLocation, fi.Name)))
                            helpers.MoveFile(Path.Combine(SourceLocation, fi.Name), Path.Combine(ArchiveLocation, fi.Name), true);
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
