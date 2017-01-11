///////////////////////////////////////////////////////////////////////////
// ResultClass.cs - This is the code which is being tested               //
//                                                                       //
// Ramakrishna Sayee, CSE681 - Software Modeling and Analysis, Fall 2016 //
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseResults
{
    public class ResultClass
    {
        public string Result(int mark)

        {
            if (mark >= 40)
            {
                return "passed";
            }
            else
            {
                return "failed";
            }

        }
        static void Main(string[] args)
        {
            ResultClass res = new ResultClass();
            String s = res.Result(40);
            Console.WriteLine("\nThe result is\n" + s);
             
        }
    }
}
