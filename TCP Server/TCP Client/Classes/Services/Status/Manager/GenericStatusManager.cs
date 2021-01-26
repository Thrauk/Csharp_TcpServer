using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Client.Classes.Services.Status.Manager
{
    class GenericStatusManager : StatusManager
    {
        public GenericStatusManager(Client client) : base(client)
        {
        }
    }
}
