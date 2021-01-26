using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Client.Classes.Services.Status.Conditions
{
    abstract class StatusCondition
    {
        protected readonly Client client;
        //TEST

        public StatusCondition(Client client)
        {
            this.client = client;
        }

        public abstract bool CheckCondition();

        protected abstract void FailedCondition();
    }
}
