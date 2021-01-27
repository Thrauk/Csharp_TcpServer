using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCP_Server.TCP_Client.Classes.Services;

namespace TCP_Server.Client.Classes
{
    class ArduinoClient : TCP_Client.Classes.Client
    {
        public ArduinoClient(TcpClient clientListener) :
            base(clientListener)
        {
            dataSender = new GenericDataSender(this);
        }
    }
}
