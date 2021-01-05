using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;
using System.Net.NetworkInformation;

namespace TCP_Server
{
    partial class TcpServer
    {

        List<string> commands = new List<string>(new string[] { "Door actioned!", "Door released!" });

        private TcpListener tcpListener;
        //private Thread listenThread;
        private readonly object listLock = new object();
        private readonly object listenLock = new object();
        //List<ClientTcp> clientsList = new List<ClientTcp>();
        Queue<ClientTcp> clientsQueue = new Queue<ClientTcp>();


        Queue<ClientTcp> demoDevices = new Queue<ClientTcp>();


        
        public Queue<string> messageCommandList = new Queue<string>();
        Thread waitingThread;
        Thread connectionCheckThread;
        Thread heartBeatThread;
        Thread responseThread;
        private int nextId = 0;


        //public List<string> messageList = new List<string>();
        public List<string> messageList = new List<string>();

        List<string> statusList = new List<string>();

        public TcpServer(string ip, string port)
        {
            //this.tcpListener = new TcpListener(IPAddress.Parse(ip), int.Parse(port));
        }

        public void StartServer()
        {
            //this.tcpListener = new TcpListener(IPAddress.Parse("192.168.0.46"), 11000);
            this.tcpListener = new TcpListener(IPAddress.Any, 11000);
            this.tcpListener.Start();

            //Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));

            waitingThread = new Thread(new ThreadStart(WaitClient));
            connectionCheckThread = new Thread(new ThreadStart(CheckConnections));
            heartBeatThread = new Thread(new ParameterizedThreadStart(ClientHeartbeat));
            responseThread = new Thread(new ThreadStart(ResponseThread));

            waitingThread.Start();
            connectionCheckThread.Start();
            messageList.Add("Server started!");
            heartBeatThread.Start(5);
            responseThread.Start();

            //Console.WriteLine("\nHit enter to continue...");
            //Console.Read();
            /*this.listenThread = new Thread(new ThreadStart(ListenForClients));
            while(true)
            {
                ;
            }*/
        }


        public void StopServer()
        {
            
            lock (listenLock)
            {
                connectionCheckThread.Abort();
                //Thread.Sleep(25);
                this.tcpListener.Stop();
                nextId = 0;

                //foreach(ClientTcp client in clientsList)
                foreach (ClientTcp client in clientsQueue)
                {
                    client.StopConnection();
                    //client.getListener().Close();
                }
                waitingThread.Abort();
                responseThread.Abort();
                //clientsList.Clear();
                clientsQueue.Clear();


            }

        }

        

        
       

        public string getMessage()
        {
            string message;
            if (messageList.Count() > 0)
            {
                message = this.messageList[0];
                messageList.RemoveAt(0);
            }
            else
            {
                message = null;
            }
            return message;
        }

        public List<string> GetClientsIps()
        {
            List<string> ret = new List<string>();
            //foreach(ClientTcp client in clientsList)
            foreach (ClientTcp client in clientsQueue)
            {
                ret.Add(client.GetIp());
            }
            return ret;
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();
            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();
                string cliIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                NetworkStream clientStream = client.GetStream();
                clientStream.Write(Encoding.ASCII.GetBytes("Salut"), 0, Encoding.ASCII.GetBytes("Salut").Length);
                clientStream.Flush();
            }
        }

    }

   

}
