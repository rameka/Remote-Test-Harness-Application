////////////////////////////////////////////////////////////////////////////////
// ResultTest.cs - Define a test to run and retrieves the the test log        //
//                                                                            //
// Ramakrishna Sayee Meka, CSE681 - Software Modeling and Analysis, Fall 2016 //
////////////////////////////////////////////////////////////////////////////////
/*
*   Test driver needs to know the types and their interfaces
*   used by the code it will test.  It doesn't need to know
*   anything about the test harness.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestInterface;
using CourseResults;

namespace ResultTestDriver
{
    public class ResultTestDriver:ITest
    {
        private ResultClass code;
        StringBuilder stringBuilder = new StringBuilder();
        //-------<Test driver constructor >------
        public ResultTestDriver()
        {
            code = new ResultClass();
        }
        public static ITest create()
        {
            return new ResultTestDriver();
        }

        //-----<Performing testing for the code>------
        public bool test()
        {
           //----<Perform the test cases>------
            if (code.Result(45) == "Passed")
            {
                stringBuilder.Append("\nTest case passed for the code under Test\n");
                return true;
            }
                
            else
            {
                stringBuilder.Append("\nTest case failed for the test case\n");
                return false;
            }
                
        }
        //------<Returning the log of the test>-------------
        public string getLog()
        {
            ;
            return stringBuilder.ToString();
        }
        static void Main(string[] args)
        {
            Console.Write("\n  Local test:\n");

            ITest test = ResultTestDriver.create();

            if (test.test() == true)
                Console.Write("\n  test passed");
            else
                Console.Write("\n  test failed");
            Console.Write("\n\n");
        }

    }
}
