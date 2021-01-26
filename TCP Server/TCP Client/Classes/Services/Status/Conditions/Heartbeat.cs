using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Client.Classes.Services.Status.Conditions
{
    class Heartbeat : StatusCondition
    {
        private readonly double time;

        public Heartbeat(Client client, double time) : base(client)
        {
            this.time = time;
        }

        public override bool CheckCondition()
        {
            DateTime timeNow = DateTime.UtcNow;
            TimeSpan timeDifference;
            timeDifference = timeNow - client.GetLastMessageTime();
            if (timeDifference.TotalSeconds > time)
            {
                FailedCondition();
                return false;
            }
            return true;
        }

        protected override void FailedCondition()
        {
            client.connectionManager.StopConnection();
        }
    }
}
