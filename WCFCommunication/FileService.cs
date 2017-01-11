/////////////////////////////////////////////////////////////////////////////
//  FileServices.cs -  File Stream Service                                 //
//  Language:     C#                                                       //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
//  Reference:    Jim Fawcett, Project Help Code #4                        // 
/////////////////////////////////////////////////////////////////////////////
/*
 *   Public Interface
 *   ----------------
 *   FileTransferutility<string> clnt = new FileTransferutility<string>();
 *   clnt.uploadFile(filename);
 *   clnt.downloadFile("test.txt");
 */

/*
 *   Build Process
 *   -------------
 *   - Required files:   Service.cs, Icommunicator.cs, FileService.cs
 *   
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 20 November 2016
 *     - first release
 * 
 */

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;


namespace WCFCommunication
{
    //Class provides ultittes for file transer
    public class FileTransferutility<T>
    {
        public string name { get; set; } = typeof(T).Name;
        ICommunicator channel;
        public string ToSendPath = "..\\..\\ToSend";
        public string SavePath = "..\\..\\SavedFiles";
        int BlockSize = 1024;
        byte[] block;

        public FileTransferutility()
        {
            block = new byte[BlockSize];
        }
        //-------<Creates the channel for transmission>--------
        ICommunicator CreateServiceChannel(string url)
        {
            BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;

            BasicHttpBinding binding = new BasicHttpBinding(securityMode);
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 500000000;
            EndpointAddress address = new EndpointAddress(url);

            ChannelFactory<ICommunicator> factory
              = new ChannelFactory<ICommunicator>(binding, address);
            return factory.CreateChannel();
        }

        //------<Uploads the required files to the specified server>------
        public bool uploadFile(string filename, string url)
        {
            string fqname = Path.Combine(ToSendPath, filename);
            try
            {

                using (var inputStream = new FileStream(fqname, FileMode.Open))
                {
                    FileTransferMessage msg = new FileTransferMessage();
                    msg.filename = filename;
                    msg.transferStream = inputStream;
                    channel = CreateServiceChannel(url);
                    channel.upLoadFile(msg);
                }

                Console.Write("\n  Uploaded file \"{0}\"", filename);
                ((System.ServiceModel.Channels.IChannel)channel).Close();
                return true;
            }
            catch
            {
                Console.Write("\n  can't find \"{0}\"", fqname);
                ((System.ServiceModel.Channels.IChannel)channel).Close();
                return false;
            }
        }

        //-----------<Downloads the required File from the specified server>-----------
        public bool downloadFile(string filename, string url)
        {
            int totalBytes = 0;

            try
            {
                channel = CreateServiceChannel(url);
                Stream strm = channel.downLoadFile(filename);
                if (strm == null)
                {
                    Console.WriteLine("file {0} not present in repository", filename);
                    ((System.ServiceModel.Channels.IChannel)channel).Close();
                    return false;
                }
                string rfilename = Path.Combine(SavePath, filename);
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
                using (var outputStream = new FileStream(rfilename, FileMode.Create))
                {
                    while (true)
                    {
                        int bytesRead = strm.Read(block, 0, BlockSize);
                        totalBytes += bytesRead;
                        if (bytesRead > 0)
                            outputStream.Write(block, 0, bytesRead);
                        else
                            break;
                    }
                }
                Console.Write("\n  Received file \"{0}\" of {1} bytes ", filename, totalBytes);

            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}", ex.Message);
                ((System.ServiceModel.Channels.IChannel)channel).Close();
                return false;
            }
            ((System.ServiceModel.Channels.IChannel)channel).Close();
            return true;
        }

#if (TEST_HARNESS)
        static void Main()
        {
            Console.Write("\n  Client of SelfHosted File Stream Service");
            Console.Write("\n ==========================================\n");

            FileTransferutility<string> clnt = new FileTransferutility<string>();
            clnt.channel = CreateServiceChannel("http://localhost:8000/StreamService");
            

            
            clnt.uploadFile("TestDriverVendingMachine1");
       
            
            clnt.downloadFile("test.txt");
         
            Console.Write("\n\n  Press key to terminate client");
            Console.ReadKey();
            
            ((IChannel)clnt.channel).Close();
        }
#endif
    }
}
