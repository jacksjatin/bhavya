using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace IMIFileGeneratorOutboundScheduler
{
    public class UploadToSFTP
    {
       
        public void uploadFiletoSFTP(FileInfo fi)
        {
            using (SftpClient sftpClient = new SftpClient("host",22,"username","password"))
            {
                Console.WriteLine("Connect to server");
                sftpClient.Connect();
                Console.WriteLine("Creating FileStream object to stream a file");
                using (FileStream fs = new FileStream(fi.FullName, FileMode.Open))
                {
                    sftpClient.BufferSize = 1024;
                    sftpClient.UploadFile(fs, Path.Combine("",fi.Name));
                }
                sftpClient.Dispose();
            }
        }


    }
}
