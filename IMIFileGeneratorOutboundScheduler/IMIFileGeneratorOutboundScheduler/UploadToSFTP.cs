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
        // SftpClient sftpClient = new SftpClient(getSftpConnection("Host", "username", 8000, "publicKeyPath"));

        public static ConnectionInfo getSftpConnection(string host, string username, int port, string publicKeyPath)
        {
            return new ConnectionInfo(host, port, username, privateKeyObject(username, publicKeyPath));
        }

        private static AuthenticationMethod[] privateKeyObject(string username, string publicKeyPath)
        {
            PrivateKeyFile privateKeyFile = new PrivateKeyFile(publicKeyPath);
            PrivateKeyAuthenticationMethod privateKeyAuthenticationMethod =
                 new PrivateKeyAuthenticationMethod(username, privateKeyFile);
            return new AuthenticationMethod[] { privateKeyAuthenticationMethod };
        }


        public void uploadFiletoSFTP(FileInfo fi)
        {
            using (SftpClient sftpClient = new SftpClient(getSftpConnection("host", "userName", 22, "filePath")))
            {
                Console.WriteLine("Connect to server");
                sftpClient.Connect();
                Console.WriteLine("Creating FileStream object to stream a file");
                using (FileStream fs = new FileStream(fi.FullName, FileMode.Open))
                {
                    sftpClient.BufferSize = 1024;
                    sftpClient.UploadFile(fs, fi.Name);
                }
                sftpClient.Dispose();
            }
        }


    }
}
