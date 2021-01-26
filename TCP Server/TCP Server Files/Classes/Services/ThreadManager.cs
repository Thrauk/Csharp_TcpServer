using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.TCP_Server_Files.Classes.Threads;

namespace TCP_Server.TCP_Server_Files.Classes.Services
{
    class ThreadManager
    {
        private Queue<ServerThread> serverThreadQueue;
        public ThreadManager()
        {
            serverThreadQueue = new Queue<ServerThread>();
            ServerWaitClientsThread waitClientsThread = new ServerWaitClientsThread(TcpServer.activeServer.connectionManager.GetServerListener());
            CheckClientStatusThread checkClientStatusThread = new CheckClientStatusThread();
            serverThreadQueue.Enqueue(waitClientsThread);
            //serverThreadQueue.Enqueue(checkClientStatusThread);
        }

        public void StartServerThreads()
        {
            foreach(ServerThread thread in serverThreadQueue)
            {
                thread.StartThread();
            }
        }

        public void StopServerThreads()
        {
            foreach (ServerThread thread in serverThreadQueue)
            {
                thread.AbortThread();
            }
        }
    }
}
