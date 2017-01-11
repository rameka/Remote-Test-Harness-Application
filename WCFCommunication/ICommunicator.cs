/////////////////////////////////////////////////////////////////////////////
//  ICommunicator.cs - Peer-To-Peer Communicator Service Contract          //
//  Language:     C#                                                       //
//  Author:       Ramakrishna Sayee Meka, Syracuse University              //
//  Reference:    Jim Fawcett, Project Help Code #4                        // 
/////////////////////////////////////////////////////////////////////////////

/*
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 17 November 2016
 *     - first release
 */

using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WCFCommunication
{
    [ServiceContract]
    public interface ICommunicator
    {
        [OperationContract(IsOneWay = true)]
        void PostMessage(Message msg);

        // used only locally so not exposed as service method
        [OperationContract(IsOneWay = true)]
        void upLoadFile(FileTransferMessage msg);
        [OperationContract]
        Stream downLoadFile(string filename);

        Message GetMessage();
    }

    // The class Message is defined in CommChannelDemo.Messages as [Serializable]
    // and that appears to be equivalent to defining a similar [DataContract]

}