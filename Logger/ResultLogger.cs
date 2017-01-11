/////////////////////////////////////////////////////////////////////////////
// ResultLogger.cs - Demonstrates Logging Operation for TestHarness        //
//  Language:     C#                                                       //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
/////////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   Stores the test request in Test Harness in a structured format and then writes
 *   the Test Results and Test Logs in two seperate files based on author name and 
 *   timestamp
 * 
 *   Public Interface
 *   ----------------
 *   TestRequestLog fLog = new TestRequestLog();
 *   fLog.DisplayLog();
 *   fLog.resultToFile(xmlPath);
 *   fLog.resultLogToFile(xmlPath)
 *   string msg = bQ.deQ();
 */

/*
 *   Build Requirement
 *   -----------------
 *   - Required files:   Parser.cs, ResultLogger.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 5 October 2016
 *     - first release
 *   ver 2.0 : 21 October 2016
 *     - Adds a new feature to write the result obbect to a file
 *      
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;
using System.Collections;
using System.IO;

namespace Logger
{
    //This class gives structure to our result after performing test to the 
    //test harness and writes them to the file.
    public class TestRequestLog : MarshalByRefObject
    {

        string author;
        int countTests;
        int passedTests;
        int failedTests;
        
        public string xmlName { get; set; }
        public List<TestSuite> testDriverAndCode = new List<TestSuite>(); 

        public void AddSuite(TestSuite ts)
        {
              testDriverAndCode.Add(ts);

        }

        //------<Displays the entire log on the console>------
        public void DisplayLog()
        {
            string s = @"*********************************";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
            Console.WriteLine(s);
            s = @"Displaying the Final Log";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
            Console.WriteLine(s);
            s = @"*********************************";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
            Console.WriteLine(s);


            Console.Write("\n  Test Request Name::{0}", xmlName);
            Console.Write("\n  ====================================");

            foreach (var tsuit in testDriverAndCode)
            {
                tsuit.show();
                Console.Write("\n  {0,12}  *********{1}********", "Test Result Log:\n", tsuit.testResult);
            }


            Console.Write("\n  ===================================================");
            Console.Write("\n  ===================================================");
        }

        //-----------<Creates the file based on the specified path and writes the text in that file>
        public void writeToFile(StringBuilder Text, string path)
        {

           if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter output = File.CreateText(path))
                {
                    output.WriteLine(Text);
                    output.Close();
                }
            }
        }
        //-------<Returns the Result of the file to the Test Harness>------
        public string resultToFile(string xmlPath)
        {
            StringBuilder fstring = new StringBuilder();
            int count =0;
            fstring.AppendLine("Final Test Results");
            fstring.AppendLine("===================");
            foreach (var tsuit in testDriverAndCode)
            {
                count++;

                if (tsuit.testStatus == "Test Passed")
                    passedTests++;
                else
                    failedTests++;
               
            }
            countTests = count;
            fstring.AppendLine("Total Number of Tests"+countTests);
            fstring.AppendLine("Passed Tests:"+passedTests);
            fstring.AppendLine("Failed Tests:"+failedTests);
            string xmlFile = Path.GetFileName(xmlPath);
            string directoryPath = Path.GetDirectoryName(xmlPath);
            string fileName = xmlFile.Split('.')[0] + "_Result.txt";
            string savPath = directoryPath + @"\" + fileName;
            writeToFile(fstring,savPath);
            return savPath;

        }

        //-------<Returns the resultLog to the the test harenss>---------
        public string resultLogToFile(string xmlPath)
        {
            
            StringBuilder fstring = new StringBuilder();

            fstring.AppendLine("Final Log");
            fstring.AppendLine("===================");
            fstring.AppendLine("Test RequestName::" + xmlName);
            fstring.AppendLine("===================");
            foreach (var tsuit in testDriverAndCode)
            {
                countTests++;
                fstring.AppendLine("Test:" + countTests.ToString());
                fstring.AppendLine("\tTestAuthor:" + tsuit.author);
                author = tsuit.author;
                fstring.AppendLine("\tTestName:" + tsuit.testName);
                fstring.AppendLine("\tTimeStamp:" + tsuit.timeStamp);
                fstring.AppendLine("\tTestDriver:" + tsuit.testDriver);
                fstring.AppendLine("\tTestStatus:" + tsuit.testStatus);
                fstring.AppendLine("\tLibraries:");
                foreach (string lib in tsuit.testCode)
                {
                    fstring.AppendLine("\t\t" + lib);
                }
                fstring.AppendLine("Test Result Log:");
                fstring.AppendLine("\t\t" + tsuit.testResult);
            }
            string xmlFile = Path.GetFileName(xmlPath);
            string directoryPath = Path.GetDirectoryName(xmlPath);
            string fileName = xmlFile.Split('.')[0] + "_ResultLog.txt";
            string savPath = directoryPath + @"\" + fileName;
            writeToFile(fstring,savPath);

            return savPath;
                      
        }

    }

//---------<Test Stub>---------

#if (TEST_LOGGER)

  class TestStub
  {
    static void Main(string[] args)
    {
      Console.Write("\n  Testing The Logger Module");
      Console.Write("\n ***************************\n");
      
            List<TestSuite> testDLL = new List<TestSuite>();
            TestSuite t1 = new TestSuite();
            t1.testCode = new List<string>();
            t1.testDriver = @"MarksTestDriver.dll";
            t1.testCode.Add(@"CourseMarks.dll");
            t1.testName = @"first";
            t1.timeStamp = DateTime.Now;
            t1.author = @"Ramakrishna";
            t1.testResult = @"Test Result Passed ";
            

            TestSuite t2 = new TestSuite();
            t2.testCode = new List<string>();
            t2.testDriver = @"ResultTestDriver.dll ";
            t2.testCode.Add(@"CourseResults.dll");
            t2.testName = @"second";
            t2.timeStamp = DateTime.Now;
            t2.author = @"Ramakrishna";
            t2.testResult = @"Raised an Exception, Test Failed";
            

            TestRequestLog fLog = new TestRequestLog();
            fLog.xmlName = @"TestRequest1";
            fLog.AddSuite(t1);
            fLog.AddSuite(t2);
            fLog.DisplayLog();                      
    }
  }
#endif

}
