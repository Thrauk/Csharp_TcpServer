using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Server
{
    class ClientTcp
    {
        private TcpClient client_listener;
        private Thread clientThread;
        private NetworkStream stream = null;
        private int clientId;
        private string ipAddress;
        private DateTime lastRecievedMessageTime;
        private bool connectionStarted = false;
        private readonly object abortLock = new object();
        public readonly object clientAliveLock = new object();

        public ClientTcp(Thread clientThread, int clientId, TcpClient client_listener)
        {
            this.clientThread = clientThread;
            this.clientId = clientId;
            this.client_listener = client_listener;
            this.ipAddress = client_listener.Client.RemoteEndPoint.ToString();
        }

        public void StopConnection()
        {
            lock(abortLock)
            {
                this.clientThread.Abort();
                this.client_listener.Close();
                if (stream != null)
                    this.stream.Close();
            }
        }

        public void SetNetworkStream(NetworkStream stream)
        {
            this.stream = stream;
        }

        public TcpClient getListener()
        {
            return this.client_listener;
        }

        public void setListener(TcpClient client_listener)
        {
            this.client_listener = client_listener;
        }

        public int getId()
        {
            return this.clientId;
        }

        public void setId(int clientId)
        {
            this.clientId = clientId;
        }

        public Thread getThread()
        {
            return this.clientThread;
        }

        public void setThread(Thread clientThread)
        {
            this.clientThread = clientThread;
        }

        public string GetIp()
        {
            return this.ipAddress;
        }

        public void SetLastMessageTime(DateTime time)
        {
            lastRecievedMessageTime = time;
        }

        public DateTime GetLastMessageTime()
        {
            return lastRecievedMessageTime;
        }

        public NetworkStream GetStream()
        {
            return this.stream;
        }

        public void SetConnectionStarted(bool value)
        {
            connectionStarted = value;
        }

        public bool GetConnectionStarted()
        {
            return connectionStarted;
        }

        public object GetAliveLock()
        {
            return clientAliveLock;
        }
    }
}
