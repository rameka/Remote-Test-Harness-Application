/////////////////////////////////////////////////////////////////////////////
//  TestInterface.cs - Test Drivers should Implement this interface        //
//                       to execute in TestHarness                         //
//  Language:     C#,                                                      //
//  Application:  Test Harness                                             //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
/////////////////////////////////////////////////////////////////////////////

/* Public Interface
 * ----------------
 * ITest having functions test(), getLog().
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestInterface
{
    public interface ITest
    {
        bool test();
        string getLog();
    }
}
