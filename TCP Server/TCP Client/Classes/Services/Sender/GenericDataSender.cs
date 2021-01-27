using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Client.Classes.Services
{
    class GenericDataSender : DataSender
    {
        
        public GenericDataSender(Client client):base(client)
        {
            acknowledgeMessage = "ACK_Gen";
        }

        public override void SendMessage(byte[] message)
        {
            client.GetClientStream().Write(message, 0, message.Length);
        }

        public override void SendMessage(string message)
        {
            SendMessage(StringToByte(message));
        }

        public override void SendCommandString(string command) { }

        public override void SendCommandBytes(byte[] command) { }

       
    }
}
