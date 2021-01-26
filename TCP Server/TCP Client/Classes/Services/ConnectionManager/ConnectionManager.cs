using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Client.Classes.Services.ConnectionManager
{
    abstract class ConnectionManager
    {
        protected Client client;


        public virtual void StartConnection()
        {
            client.GetClientThread().StartThread();
        }

        /*Don't forget to stop threads in StopConnection*/
        public virtual void StopConnection()
        {
            client.GetClientThread().AbortThread();
            client.GetClientListener().Close();
            if (client.GetClientStream() != null)
            {
                client.GetClientStream().Close();
            }
            //Tell server that client closed
            TCP_Server_Files.Classes.TcpServer.activeServer.RemoveClient(client);
        }

        public virtual bool CheckConnection()
        {
            IPGlobalProperties ipProperties;
            TcpConnectionInformation[] tcpConnections;
            try
            {
                ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                tcpConnections = ipProperties.GetActiveTcpConnections()
                    .Where(
                            x => x.LocalEndPoint.Equals(client.GetClientListener().Client.LocalEndPoint) 
                            && 
                            x.RemoteEndPoint.Equals(client.GetClientListener().Client.RemoteEndPoint)
                        ).ToArray();
                if (tcpConnections != null && tcpConnections.Length > 0)
                {
                    TcpState stateOfConnection = tcpConnections.First().State;
                    if (stateOfConnection != TcpState.Established)
                    {
                        TCP_Server_Files.Classes.TcpServer.activeServer.AddMessage("Client was disconnected because of connection failure");
                        StopConnection();
                        return false;
                    }
                    else
                    {
                        // Connection is OK
                        return true;
                    }

                }
                else
                {
                    return false; // Connection is not enstablished
                }
            }
            catch //Client had an error or disconnected
            {
                TCP_Server_Files.Classes.TcpServer.activeServer.AddMessage("Client was disconnected because of connection failure");
                StopConnection();
                return false;
            }
        }
    }
}
