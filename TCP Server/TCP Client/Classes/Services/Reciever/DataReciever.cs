using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.Client.Classes;

namespace TCP_Server.TCP_Client.Classes.Services.Reciever
{
    class DataReciever
    {

        public static string ReadData(Client client)
        {
            byte[] streamDataBytes = new Byte[256];
            int streamDataLength;
            try
            {
                streamDataLength = client.GetClientStream().Read(streamDataBytes, 0, streamDataBytes.Length);
            }
            catch
            {
                ClientStoppedResponding(client);
                return "";
            }
            if(streamDataLength == 0)
            {
                ClientStoppedResponding(client);
                return "";
            }
            client.SetLastMessageTime(DateTime.UtcNow);
            return DecodeData(BytesToString(streamDataBytes, streamDataLength));

        }


        private static string BytesToString(byte[] bytes, int length)
        {
            return Encoding.ASCII.GetString(bytes, 0, length);
        }

        private static void ClientStoppedResponding(Client client)
        {
            client.connectionManager.StopConnection();
        }
        private static string DecodeData(string message)
        {
            return message;
        }
    }

}
