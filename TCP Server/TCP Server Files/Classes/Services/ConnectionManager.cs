using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Server_Files.Classes.Services
{
    class ConnectionManager
    {
        private readonly TcpListener serverTcpListener;
        private readonly TcpServer server;

        public ConnectionManager(string listenerAddress, int portNumber)
        {
            serverTcpListener = new TcpListener(IPAddress.Any, portNumber);
            server = TcpServer.activeServer;
        }

        public void StartServer()
        {
            serverTcpListener.Start();
            server.threadManager.StartServerThreads();
        }

        public void StopServer()
        {
            serverTcpListener.Stop();
            server.threadManager.StopServerThreads();
            foreach(TCP_Client.Classes.Client client in server.GetClientsQueue())
            {
                client.connectionManager.StopConnection();
            }
            server.ClearQueue();
        }

        public TcpListener GetServerListener() => serverTcpListener;
    }
}
