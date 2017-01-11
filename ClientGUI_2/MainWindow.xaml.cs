/////////////////////////////////////////////////////////////////////////////
//  MainWindow.xaml.cs - Client GUI for test harness Application           //
//  Language:     C#                                                       //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
/////////////////////////////////////////////////////////////////////////////

/*
 *   Build Requirements
 *   ------------------
 *   - Required files:  MainWindow.xaml.cs, Client.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 21 November 2016
 *     - first release
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using WCFCommunication;
using System.Xml.Linq;
using System.Windows.Threading;
using HTimer;
using System.Threading;

namespace ClientGUI2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fpath;
        Client client;
        string timeStamp;
        string author;
        HiResTimer htr = new HiResTimer();
        Action<string> ResultDisplay { get; set; }

        public string testResult { get; set; }
       

        [DllImport("Kernel32")]
        public static extern void AllocConsole();
        [DllImport("kernel32")]
        public static extern void FreeConsole();
        Action<string, string> AT { get; set; } 
        //----------<Constructor creates client>-------       
        public MainWindow()
        {
            
            InitializeComponent();
            AllocConsole();

            Console.Title = "Client 2";
            client = new Client();
            AT = (x, y) => getFields(x, y);
            ResultDisplay = ((x) => updateTextBlock(x));
            client.DisRes = ResultDisplay;

            fpath = @"..\..\TestFolder";
            Thread Th = new Thread(() => client.TestMethod(@"..\..\TestFolder",AT));
            Th.Start();
        }

        void getFields(string _auth, string _timestamp)
        {
            author = _auth;
            timeStamp = _timestamp;
        }

        //--------------<Function for updating text box>-------------------
        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            fpath = dialog.SelectedPath;
            textBox.Text = fpath;
        }
        //--------------<Function for Execute Test button>-------------------
        private void button1_Click(object sender, RoutedEventArgs e)
        {

            htr.Start();
            textBlock.Text = "";
            string[] files;
            files = Directory.GetFiles(fpath, @"*.xml", SearchOption.TopDirectoryOnly);
            if (files.Length > 1 || files.Length < 0)
            {
                textBlock.Text = "Multiple Test Requests Exist or No file for test Request Exists, please place only one test request in the specified directory ";
                return;
            }
            XDocument doc_ = XDocument.Load(files[0]);
            if (doc_ == null)
                textBlock.Text = "No content in XML";

            author = doc_.Descendants("author").First().Value;

            string xmlPath = files[0];
            string xmlString = System.IO.File.ReadAllText(xmlPath);


            string[] filesDLL;
            filesDLL = Directory.GetFiles(fpath, @"*.dll", SearchOption.TopDirectoryOnly);
            client.FileTransfer.ToSendPath = fpath;
            Console.WriteLine("\n---Uploading Files to the repository from client from the specified path:" + System.IO.Path.GetFullPath(fpath) + "-----");
            foreach (string s in filesDLL)
            {

                client.FileTransfer.uploadFile(System.IO.Path.GetFileName(s), client.RepositoryEndPoint);
            }

            string remoteEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8080);
            Console.WriteLine("\n\n-----Sending a Test Request message from the client having the Test Tequest Name: " + System.IO.Path.GetFileName(files[0]) + "--------");
            WCFCommunication.Message msg = client.makeMessage(author, client.endPoint, remoteEndPoint, "TestRequest", xmlString);
            Console.WriteLine("\n-----Client sent the message to Test Harness Having the message structure as follows-------");
            msg.showMsg();
            timeStamp = msg.time.ToString("HH-mm-ss-fff");
            client.comm.sndr.PostMessage(msg);

        }
        //------------<Function for Result Log>---------
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.FileTransfer.SavePath = fpath;
                string resultLogFile = author + timeStamp + "_ResultLog.txt";
                Console.WriteLine("------Repository supports client Queries for test Results and test result logs by retrieving files from repository-----");
                Console.WriteLine("------Retrieving the Result Log file from the Repository on clients Request having file name:" + resultLogFile + "--------");
                client.FileTransfer.downloadFile(resultLogFile, client.RepositoryEndPoint);
                string displayLog = System.IO.File.ReadAllText(fpath + @"\" + resultLogFile);

                if (!File.Exists(fpath + @"\" + resultLogFile))
                {
                    using (StreamWriter output = File.CreateText(fpath + @"\" + resultLogFile))
                    {
                        output.WriteLine(displayLog);
                        output.Close();
                    }

                }

                System.Diagnostics.Process.Start(fpath + @"\" + resultLogFile);
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(this, "Result logs were generated as DLL files were missing in the specified test request","Log Failure", MessageBoxButton.OK);
                
            }
        }
        
 
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        //-----------<Function used in delegate>----------
        public void updateTextBlock(string x)
        {
            htr.Stop();
            string timerStr = x+$"Total time Taken for Execution: {htr.ElapsedMicroseconds} microseconds";
            Console.WriteLine("\n-----Total time Taken for test execution and Communication Latency::" + htr.ElapsedMicroseconds + "------");
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => textBlock.Text = timerStr));
        }
    }

}
