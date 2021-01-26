using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.TCP_Client.Classes.Services.Status.Conditions;

namespace TCP_Server.TCP_Client.Classes.Services.Status.Manager
{
    class ArduinoStatusManager : StatusManager
    {
        public ArduinoStatusManager(Client client, double heartbeatTime) : base(client)
        {
            conditionsQueue.Enqueue(new Heartbeat(client, heartbeatTime));
        }
    }
}
