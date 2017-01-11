/////////////////////////////////////////////////////////////////////////////
//  Repository.cs - Stores the test results and test DLL's                 //
//  Language:     C#                                                       //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
//  Reference:    Jim Fawcett, Project Help Code #4                        // 
/////////////////////////////////////////////////////////////////////////////

/*
 *   Build Requirement
 *   -----------------
 *   - Required files:   Repository.cs, WCFCommunication Package
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
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WCFCommunication;

namespace Repository
{
    //Repository class for file storage in seperate server
    class Repository
    {
        public FileTransferutility<Repository> FileTransfer { get; set; } = new WCFCommunication.FileTransferutility<Repository>();

        public Comm<Repository> comm { get; set; } = new Comm<Repository>();

        public string endPoint { get; } = Comm<Repository>.makeEndPoint("http://localhost", 8083);
        public string ClientEndPoint { get; } = Comm<Repository>.makeEndPoint("http://localhost", 8081);

        private Thread rcvThread = null;

        //----< initialize receiver >------------------------------------

        public Repository()
        {
            Console.WriteLine("------Creating a WCF communication channel for message passing for Repository--------");
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }
        //----< join receive thread >------------------------------------

        public void wait()
        {
            rcvThread.Join();
        }
        //----< construct a basic message >------------------------------

        public Message makeMessage(string author, string fromEndPoint, string toEndPoint, string msgBody)
        {
            Message msg = new Message();
            msg.author = author;
            msg.type = "Status";
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            msg.body = msgBody;
            comm.sndr.PostMessage(msg);
            return msg;
        }

 
        //----< use private service method to receive a message >--------

        void rcvThreadProc()
        {
            while (true)
            {
               
                Message msg = comm.rcvr.GetMessage();
                Console.Write("\n  {0} received message:", comm.name);
                msg.showMsg();
         
            }
        }


        static void Main()
        {
            Console.Title = "Repository";
            Console.Write("\n REPOSITORY SERVER ");
            Console.Write("\n ====================\n");

            Repository Rep = new Repository();

            Console.Write("\n\n  Press key to terminate Repository");
            Console.ReadKey();
            Console.Write("\n\n");

        }
    }
}
