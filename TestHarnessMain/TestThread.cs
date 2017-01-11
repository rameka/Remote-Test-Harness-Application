/////////////////////////////////////////////////////////////////////////////
//  TestThread.cs - Test Harness execution runs in seperate thread which   //
//                  uses the below functions on thread                     // 
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
using System.IO;
using Logger;
using System.Security.Policy;
using System.Runtime.Remoting;
using TestAppDomain;
using System.Xml.Linq;
using WCFCommunication;

namespace TestHarnessMain
{
    class TestThread
    {
        public FileTransferutility<TestHarness> FileTransfer { get; set; } = new WCFCommunication.FileTransferutility<TestHarness>();
        public string RepositoryEndPoint { get; } = Comm<TestHarness>.makeEndPoint("http://localhost", 8083);
        public bool downloadRequiredFiles(string LocalDLLPath, string XMLFile, ref string missingFile)
        {
            XDocument Document;

            try
            {
                Document = XDocument.Load(XMLFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught Exception in the XMLParser class {0}", e.Message);
                return false;
            }

            XElement[] xtests = Document.Descendants("test").ToArray();

            FileTransfer.SavePath = LocalDLLPath;
            // Iterates and finds all the testcode dll's available
            for (int i = 0; i < xtests.Count(); ++i)
            {
                string TestDriver = xtests[i].Element("testDriver").Value;
                // download the testdriver from repository
                bool FileAvailable = FileTransfer.downloadFile(TestDriver, RepositoryEndPoint);
                if (FileAvailable == false)
                {
                    missingFile = TestDriver;
                    return false;
                }

                IEnumerable<XElement> xtestCode = xtests[i].Elements("library");

                // Iterates and finds test code dll names in the xml
                foreach (var xlibrary in xtestCode)
                {
                    //Downloads the DLL's from xml file
                    FileAvailable = FileTransfer.downloadFile(xlibrary.Value.ToString(), RepositoryEndPoint);
                    if (FileAvailable == false)
                    {
                        missingFile = xlibrary.ToString();
                        return false;
                    }
                }
            }
            return true;
        }
        public static void startProcessing(string xmlReqPath, Action<string, string> PostResult)
        {
            string[] XMLFiles = Directory.GetFiles(xmlReqPath, "*.xml");
            List<TestRequestLog> requestLog = new List<TestRequestLog>();
            TestRequestLog resultObj = new TestRequestLog();
            AppDomain childDomain = null;//Creation of Child App Domain
            TestThread THM = new TestThread();
            string missingFile = ""; // Download all the required DLL's from the repository
            Console.WriteLine("\n------Downlading the Required file from the Rpository using asnchronous message passing channel. File Transfer is happening through streams of bytes------");
            bool status = THM.downloadRequiredFiles(xmlReqPath, XMLFiles[0], ref missingFile);
            if (status == false)
            {
                Console.WriteLine($"\n------Test Harness error message for DLL {missingFile} missing in the Repository------");
                PostResult("Notification", $"DLL File {missingFile}  is not available in repository");
                return;
            }
            try
            {
                AppDomain main = AppDomain.CurrentDomain;
                Console.WriteLine("\nThis is the Main AppDomain where Child AppDomain is being created {0} : ", main.FriendlyName);
                AppDomainSetup domainInfo = new AppDomainSetup();  //Setup info for the Domain
                domainInfo.ApplicationBase = "file:///" + System.Environment.CurrentDirectory;
                Evidence adEvidence = AppDomain.CurrentDomain.Evidence; //creating evidence for the Domain
                string s = Path.GetFileName(xmlReqPath);
               Console.WriteLine("\n--------Creating and Loading child Appdomain where the test is being Executed----------\n"+ "\nFor TestRequest " + s + " a seperate child AppDomain is created\n ");
               childDomain = AppDomain.CreateDomain("ChildAppDomain", adEvidence, domainInfo); //Creating  Child App domain
               childDomain.Load("TestAppDomain");
               ObjectHandle oh = childDomain.CreateInstance("TestAppDomain", "TestAppDomain.TestController"); //Creating class handle for the assembly loaded in to the child AppDomain 
               TestController testController = (TestController)oh.Unwrap();
               testController.ExecuteTest(XMLFiles[0], resultObj);//Dequing one XML request and passing it to child AppDomain 
                requestLog.Add(resultObj); //Logging the Object in a class form logger module
               Console.WriteLine("\nTest Harness is supporting the Client Queries by Storing the following logs in files for Repository");
               resultObj.DisplayLog();    //Displaying log for single xml Request After Processing
               Console.WriteLine("Test Harness is Storing the Test results in a seperate file and Test Logs in another File");
               string reslog = resultObj.resultLogToFile(XMLFiles[0]);
               string res = resultObj.resultToFile(XMLFiles[0]);
               THM.FileTransfer.ToSendPath = xmlReqPath;
               Console.WriteLine("Test Harness is uploading the Test result file and Test result log to the Repository with author name and date time stamp");
               Console.WriteLine("Test Result file Name:"+ Path.GetFileName(reslog)+ "\nTest Result Log file Name:" + Path.GetFileName(res));
               THM.FileTransfer.uploadFile(Path.GetFileName(reslog), THM.RepositoryEndPoint);
               THM.FileTransfer.uploadFile(Path.GetFileName(res), THM.RepositoryEndPoint);
               PostResult("TestResult", System.IO.File.ReadAllText(res));
                Console.WriteLine("-----Unloading the App Domain responsible for testing-----"); 
               AppDomain.Unload(childDomain); // Unloading child AppDomain
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Test Harness {0}", e.Message);
                Environment.Exit(0);
            }
        }
// Test Stub is not required because main is execcuting the function.
    }
}
