/////////////////////////////////////////////////////////////////////////////////////////
//  Parser.cs : Performs Parsing for XML files from a Repository                       //                                                  //
//  Language:     C#                                                                   //
//  Application:  Used in Test Harness application for Parsing the Test Requests       //
//                which is in XML Format                                               //
//  Author:       Ramakrishna Sayee Meka, Syracuse University, Reference::Jim Fawcett  //                          
/////////////////////////////////////////////////////////////////////////////////////////

/* 
 *   Public Interface
 *   ----------------
 *   XMLParser xml = new XMLParser();
 *   List<TestSuite> ts = xml.parse(xmlFile)
 */

/*
 *   Build Requirement
 *   -----------------
 *   - Required files:   Parser.cs
 *    
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 5th October 2016
 *     - first release
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Parser
{
    //-------<This class gives the structure for Test Drivers and Test Code in XML file>---------------
    public class TestSuite : MarshalByRefObject
    {
        public string testRequest { get; set; }
        public string testName { get; set; }
        public string author { get; set; }
        public DateTime timeStamp { get; set; }
        public String testDriver { get; set; }
        public List<string> testCode { get; set; }

        public string testStatus { get; set; }//This attribute is used only after performing test for individual drivers
        public string testResult { get; set; } //This attribute also is used only after performing test for individual drivers 

        //----<This is a default constructor the Test Suite Class>------
        public TestSuite()
        {

        }

        //---<This constructor takes test request path and assigns the test file name to testRequest property>------
        public TestSuite(string _requestpath)
        {

            testRequest = Path.GetFileName(_requestpath);

        }

        //----<This method displays all the attributes of the TestSuite class>
        public void show()
        {
            Console.Write("\n  {0,12} : {1}", "test name", testName);
            Console.Write("\n  {0,12} : {1}", "author", author);
            Console.Write("\n  {0,12} : {1}", "time stamp", timeStamp);
            Console.Write("\n  {0,12} : {1}", "test driver", testDriver);
            Console.Write("\n  {0,12} : {1}", "test name", testStatus);

            foreach (string library in testCode)
            {
                Console.Write("\n  {0,12} : {1}", "library", library);
            }
        }
    }

    //---<The following class parses the XML file using the TestSuite class object>---
    public class XMLParser : MarshalByRefObject
    {
        private XDocument doc_;
        private List<TestSuite> testList_;
        public XMLParser()
        {
            doc_ = new XDocument();
            testList_ = new List<TestSuite>();
        }

        //----<Parses the XML File and returns the each test suite in XML File>----
        public List<TestSuite> parse(System.IO.Stream xml)
        {
            TestSuite test = null;
            doc_ = XDocument.Load(xml);
            if (doc_ == null)
                return testList_;
            string author = doc_.Descendants("author").First().Value;


            XElement[] xtests = doc_.Descendants("test").ToArray();
            int numTests = xtests.Count();
            //Assigning the values to the test suite
            for (int i = 0; i < numTests; ++i)
            {
                test = new TestSuite();
                test.testCode = new List<string>();
                test.author = author;
                test.timeStamp = DateTime.Now;
                test.testName = xtests[i].Attribute("name").Value;
                test.testDriver = xtests[i].Element("testDriver").Value;
                IEnumerable<XElement> xtestCode = xtests[i].Elements("library");
                foreach (var xlibrary in xtestCode)
                {
                    test.testCode.Add(xlibrary.Value);
                }
                testList_.Add(test);

            }
            return testList_;
        }
    }

    //Following is the test stub program for the Parser package
#if (TEST_PARSER)

  class TestStub
  {
    static void Main(string[] args)
    {
      Console.Write("\n  Testing The Parser Module");
      Console.Write("\n ***************************\n");

    //Change the path string value to your local system path where the xml files are present

      string path = @"D:\SU_ SEM 1\SMA\SMA-681 Project 2 Test Harness\TestHarness\RepositoryFolder\TestRequest 1.xml";
      TestSuite testReq = new TestSuite(path);
            Console.Write("\nTest Request Name:{0}", testReq.testRequest);
            Console.Write("\n ======================================");
            XMLParser xml = new XMLParser();
            System.IO.FileStream xmlFile = new System.IO.FileStream(path, System.IO.FileMode.Open);
            List<TestSuite> ts = xml.parse(xmlFile);
           foreach(var testIterator in ts)
            {
                testIterator.show();
            }
                    
    }
  }
#endif
}
