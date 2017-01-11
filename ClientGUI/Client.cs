/////////////////////////////////////////////////////////////////////////////
//  Client.cs - Client for test harness Application                        //
//  Language:     C#                                                       //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
//  Reference:    Jim Fawcett, Project Help Code #4                        // 
/////////////////////////////////////////////////////////////////////////////

/*
 *   Public Interface
 *   ----------------
 *    Client client = new Client();
 *    client.TestMethod();
 */

/*
 *   Build Requirements
 *   ------------------
 *   - Required files:   Client.cs, HiResTimer.cs, MainWindow.Xaml.cs, WCFCommunication Package files
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 21 November 2016
 *     - first release
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WCFCommunication;
using HTimer;
using System.IO;
using System.Xml.Linq;

namespace ClientGUI
{
    class Client
    {
        public Action<string> DisRes { get; set; }
                  
        public Comm<Client> comm { get; set; } = new Comm<Client>();
        public FileTransferutility<Client> FileTransfer { get; set; } = new WCFCommunication.FileTransferutility<Client>();
        public string RepositoryEndPoint { get; } = Comm<Client>.makeEndPoint("http://localhost", 8083);

        public string endPoint { get; } = Comm<Client>.makeEndPoint("http://localhost", 8081);
        

        private Thread rcvThread = null;


        //----< initialize receiver >------------------------------------

        public Client()
        {
            Console.WriteLine("------Graphical User Interface is constructed for client to communicate with Test Harness-------");
            Console.WriteLine("------Creating a WCF communication channel for message passing for Client--------");
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }
            
        
        //----< join receive thread >------------------------------------

        public void wait()
        {
            rcvThread.Join();
        }
        //----< construct a basic message >------------------------------
        public Message makeMessage(string author, string fromEndPoint, string toEndPoint, string msgType)
        {
            Message msg = new Message();
            msg.time = DateTime.Now;
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            msg.type = msgType;
            return msg;
        }

        //-----------<Message structure for a client>------------
        public Message makeMessage(string author, string fromEndPoint, string toEndPoint, string msgType, string msgBody)
        {
            Message msg = new Message(msgBody);
            msg.time = DateTime.Now;
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            msg.type = msgType;
            return msg;
        }
        //----< use private service method to receive a message >--------

        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                Console.Write("\n  {0} received message:", comm.name);
                if (msg.type == "TestResult" || msg.type == "Notification")
                {
                    DisRes(msg.body);
                    
                }
               

                msg.showMsg();
               
            }
        }


        //-------<Following method is used for automatic execution of the client to demonstrate requirements>---------

       public void TestMethod(string path, Action<string, string> retVal)
        {
            string author;
            HiResTimer htr = new HiResTimer();
            Console.Write("\n  Testing Client Demo");
            Console.Write("\n =====================\n");

            string AutoTestPath = path;

            htr.Start();
            string[] files;
            files = Directory.GetFiles(AutoTestPath, @"*.xml", SearchOption.TopDirectoryOnly);
            if (files.Length > 1 || files.Length < 0)
            {
                DisRes("Multiple Test Requests Exist or No Test Request Exists, please place only one test request in the specified directory ");
                return;
            }
            XDocument doc_ = XDocument.Load(files[0]);
            if (doc_ == null)
                DisRes("No content in XML");

            author = doc_.Descendants("author").First().Value;

            string xmlPath = files[0];
            string xmlString = System.IO.File.ReadAllText(xmlPath);
            
            string[] filesDLL = Directory.GetFiles(AutoTestPath, @"*.dll", SearchOption.TopDirectoryOnly);
            FileTransfer.ToSendPath = AutoTestPath;
            Console.WriteLine("\n---Uploading Files to the repository from client from the specified path:" + Path.GetFullPath(AutoTestPath) + "-----");
            foreach (string s in filesDLL)
            {

                FileTransfer.uploadFile(System.IO.Path.GetFileName(s), RepositoryEndPoint);
            }

            string remoteEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8080);
            Console.WriteLine("\n\n-----Sending a Test Request message from the client having the Test Tequest Name: " + System.IO.Path.GetFileName(files[0]) + "--------");
            WCFCommunication.Message msg = makeMessage(author, endPoint, remoteEndPoint, "TestRequest", xmlString);
            Console.WriteLine("\n-----Client sent the message to Test Harness Having the message structure as follows-------");
            msg.showMsg();
            string timeStamp = msg.time.ToString("HH-mm-ss-fff");
            comm.sndr.PostMessage(msg);

            retVal(author,timeStamp);
        }
#if (TEST_CLIENT)
        {
          static void Main(string[] args)
            {
                Console.Title = "Test Harness Server";
                Console.WriteLine("Client Server");
                Console.WriteLine("===================");
                Client client = new Client();
                client.TestMethod();
            }
        }
#endif

    }
}
