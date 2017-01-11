/////////////////////////////////////////////////////////////////////////////
//  TestExecutive.cs - Test Harness Application for concurrent clients.This// 
//                     package meet the requirements for project #4        //
//  Language:     C#                                                       //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
/////////////////////////////////////////////////////////////////////////////

/*
 *   Build Requirement
 *   -----------------
 *   - Required files:   TestExecutive.cs,TestThread.cs, WCFCommunication Package
 * 
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 17 November 2016
 *     - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFCommunication;
using System.Threading;
using System.Xml;
using System.IO;


namespace TestHarnessMain
{
    class TestHarness
    {
        
        public Comm<TestHarness> comm { get; set; } = new Comm<TestHarness>();

        public string endPoint { get; } = Comm<TestHarness>.makeEndPoint("http://localhost", 8080);
        public string RepositoryEndPoint { get; } = Comm<TestHarness>.makeEndPoint("http://localhost", 8083);

        private Thread rcvThread = null;
        
        //--------<Constructor creates channel and and runs the receiver queue in a thread>-----
        public TestHarness()
        {
            Console.WriteLine("------Creating a WCF communication channel for message passing for Test Harness--------");
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
            
        }

        //----------<Structire used for sending the message>-------
        public Message makeMessage(string author, string fromEndPoint, string toEndPoint, string msgType, string msgBody)
        {
            Message msg = new Message(msgBody);
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            msg.type = msgType;
            return msg;
        }
        //------<Transmitting the result Back to the client>--------
        void MessageTransmit(string _type, string _body, Message msg)
        {
            string temp = msg.from;
            msg.from = msg.to;
            msg.to = temp;
            msg.type = _type;
            msg.body = _body;
            Console.WriteLine("-----Sending the message back to the client having the message type:"+ msg.type+" -----");
            comm.sndr.PostMessage(msg);
        }
        //-------<Receiver queue dequeues the test requests and creates a seperate thread of execution for test harness>----------

        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                Console.Write("\n  {0} Received message:", comm.name);
                Console.WriteLine("\n-----Test request Dequeued from the receiver queue, where the test requests are enqueued by the client of Test Harness Application and ready for executing the test-----");
                msg.showMsg();
                //Converting Message mody to XML File
                Console.WriteLine("------Converting the string XML to XML file where DLL names for test drivers and test codes are present.-------");
                string xml = msg.body;
                Console.WriteLine(xml);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
               
                string AuthorTimeStamp = msg.author + msg.time.ToString("HH-mm-ss-fff");
                Console.WriteLine("------Creating a Temporary directory in Test Harness with Author Time Stamp:--------"+ AuthorTimeStamp);
                string PathToXmlFile = Path.Combine(Directory.GetCurrentDirectory(), $"..\\..\\TemporaryFiles\\{AuthorTimeStamp}");
                if (!Directory.Exists(PathToXmlFile))
                    Directory.CreateDirectory(PathToXmlFile);
                string XMLFileName = $"{PathToXmlFile}\\{AuthorTimeStamp}.xml";
                doc.Save(XMLFileName);
                Action<string, string> returnMsg = (x, y) => MessageTransmit(x, y, msg);

                // Starting the task for execution
                Console.WriteLine("Creating a task to support concurrent users usiing test harness");
                Task Thread = Task.Run(() => TestThread.startProcessing(PathToXmlFile, returnMsg));
                Thread.ContinueWith(antecedent =>
                {
                    //Console.WriteLine("Deleting the directory after test execution" + PathToXmlFile);
                    //Directory.Delete(PathToXmlFile, true);
                });

              

            }
        }

        class TestExecutive
        {
            static void Main(string[] args)
            {
                Console.Title = "Test Harness Server";
                Console.WriteLine("Test Harness Server");
                Console.WriteLine("===================");
                TestHarness TH = new TestHarness();
            }
        }
    }

}