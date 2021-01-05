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
        public ClientTcp GetClientByIp(string ip)
        {
            return clientsQueue.First(el => el.GetIp() == ip);
        }

        public void AddSubscriber(string ip)
        {
            demoDevices.Enqueue(GetClientByIp(ip));
        }

        public void DisconnectClient(String ip)
        {
            //ClientTcp client = clientsList.First(el => el.GetIp() == ip);
            ClientTcp client = clientsQueue.First(el => el.GetIp() == ip);
            client.StopConnection();
            //clientsList.Remove(client);
            clientsQueue = new Queue<ClientTcp>(clientsQueue.Where(element => element != client));
            client.getListener().Close();
            messageList.Add("Client " + client.getId().ToString() + " disconnected!");
        }

        public void SendStringToClientIp(String ip, String message)
        {
            //ClientTcp client = clientsList.First(el => el.GetIp() == ip);
            ClientTcp client = clientsQueue.First(el => el.GetIp() == ip);
            byte[] messageByte = Encoding.ASCII.GetBytes(message);
            client.GetStream().Write(messageByte, 0, messageByte.Length);
            messageList.Add("Server - Client " + client.getId().ToString() + " : " + message);
        }

    }
}
