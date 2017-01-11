////////////////////////////////////////////////////////////////////////////////
// MarkTEst.cs - Define a test to run an retrieves the the test log           //
//                                                                            //
// Ramakrishna Sayee Meka, CSE681 - Software Modeling and Analysis, Fall 2016 //
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseMarks;
using TestInterface;


namespace MarksTestDriver
{
    public class MarkTestDriver:ITest
    {
        private MarksClass code;
        StringBuilder stringBuilder = new StringBuilder();
        
        //-------<Test driver constructor >------
        public MarkTestDriver()
        {
            code = new MarksClass();
        }
        public static ITest create()
        {
            return new MarkTestDriver();
        }
        //-----<Performing testing for the code>------
        public bool test()
        {
            //Perform the test cases
            if (code.Marks(20,30,40) == 90)
            {
                stringBuilder.Append("Test case passed for the test case, marks=45");
                return true;
            }

            else
            {
                stringBuilder.Append("Test case failed for the test case, marks=45");
                return false;
            }
        }
        //------<Returning the log of the test>-------------
        public string getLog()
        {
            Console.WriteLine("\nInside Marks Test Driver and returning log");
            return stringBuilder.ToString();
        }

        static void Main(string[] args)
        {
            Console.Write("\n  Local test:\n");

            ITest test = MarkTestDriver.create();

            if (test.test() == true)
                Console.Write("\n  test passed");
            else
                Console.Write("\n  test failed");
            Console.Write("\n\n");
        }
    }
}
