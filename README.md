# Remote Test Harness Application

## Outline & Specifications

In this project, we are implementing test harness server which automates the process of integration testing. The system is implemented using C#, visual studio 2015 and the facilities provide by the .Net framework. The project should have the following specifications:

* Test harness should accept test requests form the client server and fetches the data from the repository server, where test code and test drivers are stored.
* The test code and drivers are loaded in to the repository by the client using a GUI. If a Test Request specifies test DLLs not available from the Repository, the Test Harness server will send back an error message to the client.
* Test Harness supports multiple clients to send test requests concurrently. The results for the test requests are also processed concurrently.
* To support concurrent access of multiple clients we use Threads.
* Test processing is done in AppDomain for each test request which separated from test harness.
* The unhandled exceptions should not interfere with the functioning of test harness. 
* Test harness stores the results of each test requests for the respective client in the repository server in the form of file. 
* The repository server should retrieve the test data and logs when requested by the user. 
* All the communication between Clients, Test Harness and the Repository is implemented using Windows Communication Foundation (WCF).
* The Graphical User Interface (GUI) for the Client is constructed using Windows Presentation Foundation (WPF).

## Steps to run the Application
* Use compile.bat and Run.bat to execute the application. Run as administrator to execute Run.bat and Compile.bat
* Application opens console windows for Repository, Client and Test Harness along with the GUI for clients.
* For demonstration of concurret users support two client windows get opened simultaneously aalong with their respective consoles for client, Test Harness and Repository.
* The results for the test are displayed on the GUI after execution of tests.
* To run the test, client should create a local directory consisting of test request and DLL files.The folder should not consist of more than one test request in XML format.
* client should then click the 'Execute Test' Button to perform the test. 
* To get the entire test logs client should click 'Result Log' button. The test log gets saved in the directory wher the client supplies the DLL's and test request with author and date time format. Client could Request log to repository any number of time.
* The result log gets opened in notepad as text file format.
* For demonstration purpose the two client GUI's take default inputs and run   the tests. To get the entire log for default inputs client shoulld click    the 'Result Log' button. 
* User could run his/her test after the default tests executes.

## Additional Comments

For further reference on test harness application an operational concept document (i.e. Operational Concept Document - Test Harness.pdf) is present in the repository. It consists of the system design, architecture, usecases and critical issues addressed in this application. It explains each module of the application in-depth.
