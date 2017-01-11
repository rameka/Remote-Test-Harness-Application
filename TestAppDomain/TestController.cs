/////////////////////////////////////////////////////////////////////////////////////////////
//  TestController.cs : This is the Assembly the child App Domain Loads to execute the     //
//                      each tests request in a Test Harness                               //
//  Language:           C#                                                                 //
//  Author:             Ramakrishna Sayee Meka, Syracuse University, Reference::Jim Fawcett//                          
/////////////////////////////////////////////////////////////////////////////////////////////
/*
 *   Public Interface
 *   ----------------
 *   TestController tc = new testController();
 *   tc.ExecuteTest(path, result)
 */

/*
 *   Build Process
 *   -------------
 *   - Required files:   TestThread.cs, TestController.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 5 October 2016
 *     - first release
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger;
using Parser;
using System.IO;
using Loader;


namespace TestAppDomain
{
    //Class which is being used to invoke test execution in AppDomain
    public class TestController : MarshalByRefObject
    {
        AppDomain ad = null;

        //----<Constructor for AppDomain Creation>--------
        public TestController()
        {
            ad = AppDomain.CurrentDomain;
            Console.Write("\nApplication Domain created{0}", ad.FriendlyName);

        }

        //---------<This function injects Loader to ChildAppDomain and Parsing and Logging of XML takes Place>----
        //---------<This function Executes each test request in a Child AppDomain >------------
        public void ExecuteTest(string path, TestRequestLog result)
        {
            //Opening the XML file that has to be sent to Parser module
            System.IO.FileStream xmlFile = new System.IO.FileStream(path, System.IO.FileMode.Open);
            //Getting the path of the RepositoryDirectory where the xmls are present
            string repositoryPath = Path.GetDirectoryName(path);

            TestSuite testRequest = new TestSuite(path);
            //Adding the Test Request Name to the Logger Module.
            result.xmlName = Path.GetFileName(path);

            XMLParser xml = new XMLParser();
            // Creating reference for test suites to refer the testsuites which we receive from parser 
            List<TestSuite> testDLL;

            testDLL = xml.parse(xmlFile);

            Console.WriteLine("\nTest logs are being stored by test request  in App Domin \n " + ad + "\n");
            Console.WriteLine("======================================================================================");
            Console.WriteLine("\nThe Test Harness  store test results and logs for each of the test executions using a key that combines the test developer identity and the current date-time");
            Console.WriteLine("======================================================================================");
            foreach (var td in testDLL)
            {
                Console.WriteLine("For Test Execution:" + td.testName);
                Console.WriteLine("Test author: " + td.author + "\t Date and Time :" + td.timeStamp);
            }
            Console.WriteLine("======================================================================================");
            result.testDriverAndCode = testDLL;

            TestLoad loadDriver = new TestLoad();
            //Loading Tests Suites and Executing the Tests which contain test codes and test driver  
            loadDriver.LoadExecuteAssembly(repositoryPath, testDLL);
            Console.WriteLine("======================================================================================");
            Console.WriteLine("\nThe logs for the Test " + result.xmlName + " are as Follows:");
            Console.WriteLine("======================================================================================");
            foreach (var td in testDLL)
            {
                Console.WriteLine("Test Log for " + td.testName + " is :" + td.testResult);
            }

        }

    }

#if (TEST_DOMAIN)


    class TestStub
    {

        static void Main(string[] args)
        {
            TestController tc = new TestController();
            Logger.TestRequestLog result = new Logger.TestRequestLog();
            //Set the local path for DLLs and xml file
            string path = @" D:\SU_ SEM 1\SMA\Project 4 - Test Harness\TestHarnessApplication\TestAppDomain\TemporaryFiles";
            tc.ExecuteTest(path, result);
            result.DisplayLog();
        }
    }
#endif
}
