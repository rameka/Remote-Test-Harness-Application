/////////////////////////////////////////////////////////////////////////////////////////
//  LoaderModule.cs : Performs Loading Operation for the Test Harness Application      //
//                    Loads the TestDriver and Test Code to Assemblies                 //
//  Language:     C#                                                                   //
//  Author:       Ramakrishna Sayee Meka, Syracuse University, Reference::Jim Fawcett  //                          
/////////////////////////////////////////////////////////////////////////////////////////

/*
 *   Public Interface
 *   ----------------
 *   TestLoad th = new TestLoad();
 *   th.LoadAssembly(repositoryPath, testDLL);
 *   
 */
/*
*   Build Requirement
*   -----------------
*   - Required files:   Parser.cs, LoaderModule.cs   
*  
*   Maintenance History
*   -------------------
*   ver 1.0 :  4 October 2016
*     - first release
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;
using System.Reflection;
using TestInterface;
using System.IO;

namespace Loader
{
    public class TestLoad : MarshalByRefObject
    {

        //------<This function takes multiple tests in XML file and XML path as argument to Load and Execute each test in Assemblies>------------
        public void LoadExecuteAssembly(string path, List<TestSuite> testDLL_)
        {

            foreach (var suite in testDLL_)
            {

                LoadTestSuit(path, suite);
            }

        }

        //-------------<This function actually Loads and executes each test driver and its respective codes>------
        public void LoadTestSuit(string path, TestSuite test)
        {
            Console.WriteLine("\nTest Which is being Processed in the XML File {0}: ", test.testName);

            List<string> allAssemblies = new List<string>();
            //Storing teest driver path to a string
            string testDriverFile = path + @"\" + test.testDriver;
            allAssemblies.Add(testDriverFile);
            foreach (string a in test.testCode)
            {
                string testC = path + @"\" + a;
                allAssemblies.Add(testC);

            }

            foreach (string file in allAssemblies)
            {
                String fileName = Path.GetFileName(file);
                Console.Write("\nLoading Test DLL's in XML file for Testing : \"{0}\"", fileName);

                try
                {
                    Assembly assem = Assembly.LoadFrom(file);
                    Type[] types = assem.GetExportedTypes();

                    foreach (Type t in types)
                    {
                        if (t.IsClass && typeof(ITest).IsAssignableFrom(t))  // does this type derive from ITest ?
                        {
                            Console.WriteLine("\nThe Driver implements the inteface ITest:" + t.Name);
                            
                            ITest tdr = (ITest)Activator.CreateInstance(t);    // create instance of test driver
                            
                            bool result = tdr.test();
                            if (result == true)
                                test.testStatus = @"Test Passed";
                            else
                                test.testStatus = @"Test Failed";

                            test.testResult = tdr.getLog();
                            Console.WriteLine("Logs present in the get method of the Drivers"+ tdr.getLog());
                        }

                    }

                }
                catch (Exception ex)
                {
                    Console.Write("\n  {0}", ex.Message);
                    test.testStatus = @"Test Failed";
                    test.testResult = @"\nCaught an exception for Test Driver----->" + test.testDriver + "\n";
                }
            }
            Console.Write("\n");

        }

        // Following is the Test stub for testing the package. 

#if (TEST_LOADER)

  class TestStub
  {
    static void Main(string[] args)

    {
      Console.Write("\n  Testing The Loader Module");
      Console.Write("\n ***************************\n");

    //Change the path string value to your local system path where the xml files are present

      string repositoryPath = @"D:\SU_ SEM 1\SMA\SMA-681 Project 2 Test Harness\TestHarness\RepositoryFolder\";
      TestLoad th = new TestLoad();
                List<TestSuite> testDLL = new List<TestSuite>();
                TestSuite t1 = new TestSuite();
                t1.testCode = new List<string>();
                
                t1.testDriver = @"MarksTestDriver.dll";
                t1.testCode.Add(@"CourseMarks.dll");
                t1.testName = @"first";
                
                TestSuite t2 = new TestSuite();
                t2.testCode = new List<string>();
                t2.testDriver = @"ResultTestDriver.dll ";
                t2.testCode.Add(@"CourseResults.dll");
                t2.testName = @"second";
                t2.show();
                testDLL.Add(t1);
                testDLL.Add(t2);
                th.LoadAssembly(repositoryPath, testDLL);
                t1.show();
        // If some missing attributes are found they are yet to be assigend. VAlues could beassigned to the missing attributes
                Console.WriteLine();

      
    }
  }
#endif

    }
}
