///////////////////////////////////////////////////////////////////////
///  HiResTimer.cs - High Resolution Timer - Uses Win32             ///
///  ver 1.0         Performance Counters and .Net Interop          ///
///                                                                 ///
///  Language:     Visual C#                                        ///
///  Author:       Ramakrishna Sayee, Syracuse University           ///       
///  Reference:    Jim Fawcett, Project Helper Code #4m             ///
///////////////////////////////////////////////////////////////////////
/// Based on:                                                       ///
/// Windows Developer Magazine Column: Tech Tips, August 2002       ///
/// Author: Shawn Van Ness, shawnv@arithex.com                      ///
///////////////////////////////////////////////////////////////////////
/*
 * Modular Operations
 * ------------------
 * This is module is used to find the time taken to perform a particular operation. 
 * Start the timer before starting the operation using Start() method and Stop() method
 * after the completion of operations. Retrive the time in microseconds, frequency, elapsed
 * ticks and time span.  
 * /
/*
 Public Interface
 ----------------
 HiResTimer hrt = new HiResTimer();
 hrt.start();
 hrt.stop();
 ulong microsec = hrt.ElapsedMicroseconds;
 ulong freq = hrt.Frequency;
 ulong eTicks = hrt.ElapsedTicks;
 TimeSpan ts = hrt.ElapsedTimeSpan;
 */
/*
 * Maintenance History
 * --------------------
 *   ver 1.0 : 19 November 2016
 *     - first release
 * */
using System;
using System.Runtime.InteropServices; // for DllImport attribute
using System.ComponentModel; // for Win32Exception class
using System.Threading; // for Thread.Sleep method
using System.IO;

namespace HTimer
{
    public class HiResTimer
    {
        protected ulong a, b, f;

        //----------<Constructor>------------
        public HiResTimer()
        {
            a = b = 0UL;
            if (QueryPerformanceFrequency(out f) == 0)
                throw new Win32Exception();
        }
        //---------<Finds the Elapsed Ticks>-------
        public ulong ElapsedTicks
        {
            get
            { return (b - a); }
        }

        //--------<Finds the time taken in microseconds>-----------
        public ulong ElapsedMicroseconds
        {
            get
            {
                ulong d = (b - a);
                if (d < 0x10c6f7a0b5edUL) // 2^64 / 1e6
                    return (d * 1000000UL) / f;
                else
                    return (d / f) * 1000000UL;
            }
        }

        //--------<Returns the Elapsed time>---------

        public TimeSpan ElapsedTimeSpan
        {
            get
            {
                ulong t = 10UL * ElapsedMicroseconds;
                if ((t & 0x8000000000000000UL) == 0UL)
                    return new TimeSpan((long)t);
                else
                    return TimeSpan.MaxValue;
            }
        }
        //---------<Retruns frequency>---------
        public ulong Frequency
        {
            get
            { return f; }
        }

        //-------<Starts the timer>------
        public void Start()
        {
            Thread.Sleep(0);
            QueryPerformanceCounter(out a);
        }
        //-------<Stops the Timer>------
        public ulong Stop()
        {
            QueryPerformanceCounter(out b);
            return ElapsedTicks;
        }

        // Here, C# makes calls into C language functions in Win32 API
        // through the magic of .Net Interop

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern
           int QueryPerformanceFrequency(out ulong x);

        [DllImport("kernel32.dll")]
        protected static extern
           int QueryPerformanceCounter(out ulong x);
    }

//-------<Test Stub>----------
#if (TEST_HTIMER)
    class TestStub
    {
        public static void Main()
        {
            HiResTimer hrt = new HiResTimer();
            hrt.Start();
            string path = "xyz.txt";
            string text = "This is the Temporay text file";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter output = File.CreateText(path))
                {
                    output.WriteLine(text);
                    output.Close();
                }
            }
            hrt.Stop();
            Console.WriteLine(hrt.ElapsedMicroseconds);
            Console.WriteLine(hrt.Frequency);
            Console.WriteLine(hrt.ElapsedTicks);
            Console.WriteLine(hrt.ElapsedTimeSpan);
        }   
    }
#endif
}
