using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCP_Server.TCP_Client.Classes.Services.Reciever;

namespace TCP_Server.TCP_Client.Classes.Threads
{
    class ClientRecieveDataThread : ClientThread
    {
        private readonly Client client;

        public ClientRecieveDataThread(Client client)
        {
            this.client = client;
        }

        protected override void ThreadBody()
        {
            string message;
            while(true)
            {
                
                message = DataReciever.ReadData(client);
                //SEND DATA TO SERVER
                TCP_Server_Files.Classes.TcpServer.activeServer.AddMessage(message);
            }
        }

    }
}

