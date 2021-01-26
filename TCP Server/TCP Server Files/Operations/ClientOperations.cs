using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TCP_Server.TCP_Server_Files.Classes.Operations
{
    class ClientOperations
    {

        static public TCP_Client.Classes.Client GetClientByIp(string ip)
        {
            return TcpServer.activeServer.GetClientsQueue().First(el => el.GetIp() == ip);
        }

        static public void DisconnectClientByIp(string ip)
        {
            TCP_Client.Classes.Client client;
            client = GetClientByIp(ip);
            client.connectionManager.StopConnection();
        }

        static public int NextClientId()
        {
            int id = TcpServer.activeServer.nextId;
            TcpServer.activeServer.nextId += 1;
            return id;
        }

        static public void SendStringToClientIp(string ip, string message)
        {
            //ClientTcp client = clientsList.First(el => el.GetIp() == ip);
            TCP_Client.Classes.Client client = GetClientByIp(ip);
            client.dataSender.SendMessage(message);
            TcpServer.activeServer.AddMessage("Server - Client " + client.GetId().ToString() + " : " + message);
        }

        static public List<string> GetClientsIps()
        {
            List<string> ret = new List<string>();
            foreach (TCP_Client.Classes.Client client in TcpServer.activeServer.GetClientsQueue())
            {
                ret.Add(client.GetIp());
            }
            return ret;
        }

    }
}
