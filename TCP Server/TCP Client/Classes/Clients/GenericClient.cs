using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCP_Server.TCP_Client.Classes.Services;

namespace TCP_Server.TCP_Client.Classes.Clients
{
    class GenericClient : Client
    {
        public GenericClient(TcpClient clientListener) : base(clientListener)
        {
            dataSender = new GenericDataSender(this);
        }
    }
}
