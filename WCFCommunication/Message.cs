/////////////////////////////////////////////////////////////////////////////
//  Message.cs - Defines Communication Message                             //
//  Language:     C#                                                       //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
//  Reference:    Jim Fawcett, Project Help Code #4                        // 
/////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Messages provides helper code for building and parsing XML messages.
 *
 * Required files:
 * ---------------
 * - Messages.cs
 * 
 * Maintanence History:
 * --------------------
 * ver 1.0 : 19 Nov 2016
 * - first release
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace WCFCommunication
{
    //Defining the following message contract to allow file transfers in WCF communication
    [MessageContract]
    public class FileTransferMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string filename { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream transferStream { get; set; }
    }

    // Following message format is used for communications other than file transfers.
    [Serializable]
    public class Message
    {
        public string type { get; set; } = "default";
        public string to { get; set; }
        public string from { get; set; }
        public string author { get; set; } = "";
        public DateTime time { get; set; } = DateTime.Now;
        public string body { get; set; } = "none";

        public Message()
        {
            body = "";
        }
        public Message(string bodyStr)
        {
             body = bodyStr;
        }
        

        //---------<Converting the message to string>----------
        public override string ToString()
        {
            string temp = "type: " + type;
            temp += ", to: " + to;
            temp += ", from: " + from;
            if (author != "")
                temp += ", author: " + author;
            temp += ", time: " + time;
            temp += ", body:\n" + body;
            return temp;
        }
        //-------<Copying the messsage structure>----------
        public Message copy()
        {
            Message temp = new Message();
            temp.type = type;
            temp.to = to;
            temp.from = from;
            temp.author = author;
            temp.time = DateTime.Now;
            temp.body = body;
            return temp;
        }
    }

    public static class extMethods
    {
        //------------<Displays the message when invoked>----------
        public static void showMsg(this Message msg)
        {
            Console.Write("\n  formatted message:");
            string[] lines = msg.ToString().Split(new char[] { ',' });
            foreach (string line in lines)
            {
                Console.Write("\n    {0}", line.Trim());
            }
            Console.WriteLine();
        }
    }

//class for test stub
    class TestMessages
    {
#if (TEST_MESSAGES)
    static void Main(string[] args)
    {
      Console.Write("\n  Testing Message Class");
      Console.Write("\n =======================\n");

      Message msg = new Message();
      msg.to = "http://localhost:8080/ICommunicator";
      msg.from = "http://localhost:8081/ICommunicator";
      msg.author = "Fawcett";
      msg.type = "TestRequest";
      msg.showMsg();
      Console.Write("\n\n");
    }
#endif
    }
}
