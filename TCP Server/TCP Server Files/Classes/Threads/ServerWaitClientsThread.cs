using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCP_Server.TCP_Client.Classes;
using TCP_Server.TCP_Client.Classes.Clients;
using TCP_Server.TCP_Client.Classes.Services.ClientAssigner;
using TCP_Server.TCP_Client.Classes.Services.Reciever;

namespace TCP_Server.TCP_Server_Files.Classes.Threads
{
    class ServerWaitClientsThread : ServerThread
    {
        private readonly TcpListener serverTcpListener;

        public ServerWaitClientsThread(TcpListener serverTcpListener)
        {
            this.serverTcpListener = serverTcpListener;
        }

        protected override void ThreadBody()
        {
            TcpClient clientListener;
            while (true)
            {
                Thread.Sleep(50);
                try
                {
                    clientListener = serverTcpListener.AcceptTcpClient();
                    TCP_Client.Classes.Client client = new GenericClient(clientListener);
                    /*string identityMessage = DataReciever.ReadData(client);
                    if(ClientAssigner.TypeVerify(identityMessage))
                    {
                        client = ClientAssigner.TypeAssigner(identityMessage, client);
                    }*/
                    client.connectionManager.StartConnection();
                    TcpServer.activeServer.AddMessage("Client connected!");
                    TcpServer.activeServer.AddClient(client);
                }
                catch(SocketException)
                {
                    break;
                }
                
            }
        }

    }
}
