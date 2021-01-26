using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Server_Files.Classes.Threads
{
    class CheckClientStatusThread : ServerThread
    {
        protected override void ThreadBody()
        {
            while(true)
            {
                Thread.Sleep(50);
                foreach(TCP_Client.Classes.Client client in TcpServer.activeServer.GetClientsQueue())
                {
                    /*
                    bool status = client.connectionManager.CheckConnection();
                    if (status == false)
                    {
                        break;
                    }
                    
                    status = client.statusManager.CheckConditions();
                    if (status == false)
                    {
                        break;
                    }*/
                }
            }
        }
    }
}
