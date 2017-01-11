////////////////////////////////////////////////////////////////////////////////
// MarksClass.cs - This is the code which is being tested                     //
//                                                                            //
// Ramakrishna Sayee Meka, CSE681 - Software Modeling and Analysis, Fall 2016 //
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseMarks
{
    public class MarksClass
    {
        public int Marks(int assignment, int midTerm, int finalExam)
        {
             return (assignment+midTerm+finalExam);
        }
        static void Main(string[] args)
        {
            MarksClass mar = new MarksClass();
            Console.WriteLine("Final marks for the Course"+mar.Marks(20,25,20));
            Console.Write("\n\n");
        }
    }
}
