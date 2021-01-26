using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.TCP_Client.Classes;
using TCP_Server.TCP_Server_Files.Classes.Services;

namespace TCP_Server.TCP_Server_Files.Classes
{
    class TcpServer
    {
        /* */
        public ConnectionManager connectionManager;
        public ThreadManager threadManager;
        /* */

        public int nextId = 0;

        public List<string> messageList = new List<string>();

        public static TcpServer activeServer;
        private Queue<TCP_Client.Classes.Client> clientsQueue = new Queue<TCP_Client.Classes.Client>();

        public TcpServer(string ip, int port)
        {
            activeServer = this;
            connectionManager = new ConnectionManager(ip,port);
            threadManager = new ThreadManager();
        }

        public void AddClient(TCP_Client.Classes.Client client)
        {
            clientsQueue.Enqueue(client);
        }

        public void RemoveClient(TCP_Client.Classes.Client client)
        {
            clientsQueue = new Queue<TCP_Client.Classes.Client>(clientsQueue.Where(element => element != client));
        }

        public string GetMessage()
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

        public void AddMessage(string message)
        {
            messageList.Add(message);
        }

        public void ClearQueue()
        {
            clientsQueue.Clear();
        }

        public Queue<TCP_Client.Classes.Client> GetClientsQueue()
        {
            return clientsQueue;
        }
        
    }
}
