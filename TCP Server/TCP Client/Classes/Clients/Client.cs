using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCP_Server.TCP_Client.Classes.Services;
using TCP_Server.TCP_Client.Classes.Services.ConnectionManager;
using TCP_Server.TCP_Client.Classes.Services.Status.Manager;
using TCP_Server.TCP_Client.Classes.Threads;

namespace TCP_Server.TCP_Client.Classes
{
    abstract class Client
    {
        /*Services*/
        public DataSender dataSender;
        public ConnectionManager connectionManager;
        public StatusManager statusManager; 
        /*End Services*/


        /*Threads */
        protected readonly ClientRecieveDataThread recieveDataThread;
        /*End Threads */


        /*Connection variables*/
        protected NetworkStream stream = null;
        protected readonly int clientId;
        protected readonly string ipAddress;
        protected DateTime lastRecievedMessageTime;
        protected bool connectionStarted = false;
        /*End Connection variables*/

        
        protected readonly TcpClient clientListener;

        protected string type = "default";
        
        

        public Client(TcpClient clientListener)
        {
            this.clientId = TCP_Server_Files.Classes.Operations.ClientOperations.NextClientId(); 
            this.clientListener = clientListener;
            ipAddress = clientListener.Client.RemoteEndPoint.ToString();
            dataSender = new GenericDataSender(stream);
            connectionManager = new GenericConnectionManager(this);
            recieveDataThread = new ClientRecieveDataThread(this);
            statusManager = new GenericStatusManager(this);
        }



        public void Identify() { }
        public void GetClientType() { }
        public void ExecFeature() { }

        public ClientRecieveDataThread GetClientThread() => recieveDataThread;
        public TcpClient GetClientListener() => clientListener;
        public DateTime GetLastMessageTime() => lastRecievedMessageTime;
        public string GetIp() => ipAddress;
        public int GetId() => clientId;

        public NetworkStream GetClientStream()
        {
            NetworkStream stream;
            try
            {
                stream = clientListener.GetStream();
                this.stream = stream;
            }
            catch
            {
                stream = null;
            }
            return stream;
        }

        public void SetLastMessageTime(DateTime time)
        {
            lastRecievedMessageTime = time;
        }


    }
}
