using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.TCP_Client.Classes.Services.Status.Conditions;

namespace TCP_Server.TCP_Client.Classes.Services.Status.Manager
{
    abstract class StatusManager
    {
        protected readonly Client client;
        protected Queue<StatusCondition> conditionsQueue;

        public StatusManager(Client client)
        {
            this.client = client;
            conditionsQueue = new Queue<StatusCondition>();
        }

        public bool CheckConditions()
        {
            foreach(StatusCondition condition in conditionsQueue)
            {
                if(condition.CheckCondition() == false)
                {
                    //Tell server which condition failed
                    return false;
                }
            }
            return true;
        }


    }
}
