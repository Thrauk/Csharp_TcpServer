using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;
using System.Net.NetworkInformation;

namespace TCP_Server
{
    class TcpServer
    {
        private TcpListener tcpListener;
        //private Thread listenThread;
        private readonly object listLock = new object();
        private readonly object listenLock = new object();
        List<ClientTcp> clientsList = new List<ClientTcp>();
        Thread waitingThread;
        Thread connectionCheckThread;
        Thread heartBeatThread;
        private int nextId = 0;


        public List<string> messageList = new List<string>();
        List<string> statusList = new List<string>();

        public TcpServer(string ip, string port)
        {
            //this.tcpListener = new TcpListener(IPAddress.Parse(ip), int.Parse(port));
        }

        public void StartServer()
        {
            this.tcpListener = new TcpListener(IPAddress.Parse("192.168.0.46"), 11000);
            this.tcpListener.Start();

            //Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));

            waitingThread = new Thread(new ThreadStart(WaitClient));
            connectionCheckThread = new Thread(new ThreadStart(CheckConnections));
            heartBeatThread = new Thread(new ParameterizedThreadStart(ClientHeartbeat));
            

            waitingThread.Start();
            connectionCheckThread.Start();
            messageList.Add("Server started!");
            heartBeatThread.Start(5);


            //Console.WriteLine("\nHit enter to continue...");
            //Console.Read();
            /*this.listenThread = new Thread(new ThreadStart(ListenForClients));
            while(true)
            {
                ;
            }*/
        }


        public void StopServer()
        {
            
            lock (listenLock)
            {
                connectionCheckThread.Abort();
                //Thread.Sleep(25);
                this.tcpListener.Stop();
                nextId = 0;
            
                foreach(ClientTcp client in clientsList)
                {
                    client.StopConnection();
                    //client.getListener().Close();
                }
                waitingThread.Abort();
                clientsList.Clear();


            }

        }

        public void StartUpdate(RichTextBox messageDisplay)
        {
            Thread updateThread = new Thread(new ParameterizedThreadStart(UpdateData));
            updateThread.Start(messageDisplay);
        }


        private void UpdateData(object argMessageDisplay)
        {
            RichTextBox messageDisplay = (RichTextBox)argMessageDisplay;
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            while (true)
            {
                if(messageList.Count() > 0)
                {
                    foreach (string message in messageList)
                    {
                        messageDisplay.AppendText(message);
                    }
                }
            }

        }

        private void WaitClient()
        {
            ClientTcp client;
            while (true)
            {
                statusList.Add("Waiting for a connection... ");
                //Console.Write();

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                



                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                try
                {
                    client = new ClientTcp(clientThread, nextId, this.tcpListener.AcceptTcpClient());
                    nextId++;                    
                }
                catch(SocketException)
                {
                    return;
                }
                lock (listLock)
                {
                    // adaug clientul in lista de clienti conectati
                    clientsList.Add(client);
                }

                clientThread.Start(client);

                // Shutdown and end connection
                //client.Close();
            }
        }

        private void HandleClient(object clientArg)
        {
            ClientTcp clientObj = (ClientTcp)clientArg;
            //Tuple<int, TcpClient> tuple = (Tuple<int, TcpClient>)clientTuple;
            int id = clientObj.getId();
            TcpClient client = clientObj.getListener();

            Byte[] bytes = new Byte[256];
            String data = null;

            //Console.WriteLine("Connected client with id " + id.ToString() + "!");

            messageList.Add("Connected client with id " + id.ToString() + "!");

            data = null;

            //while (true)
            //{
            // Get a stream object for reading and writing
            NetworkStream stream;
            
            try {
                stream = client.GetStream();
                clientObj.SetNetworkStream(stream);
            }
            catch {
                stream = null;
            }
            if (stream != null)
            {

                int i = 0;
                while (true)
                {
                    // Loop to receive all the data sent by the client.
                    try
                    {
                        i = stream.Read(bytes, 0, bytes.Length);

                    }
                    catch
                    {
                        clientObj.StopConnection();
                        clientsList.Remove(clientObj);
                        client.Close();
                        messageList.Add("Client " + clientObj.getId().ToString() + " disconnected!");
                    }
                    if(i == 0)
                    {
                        clientObj.StopConnection();
                        clientsList.Remove(clientObj);
                        client.Close();
                        messageList.Add("Client " + clientObj.getId().ToString() + " disconnected!");
                    }
                    else
                    {
                        

                        //Decode recieved message from bytes to string
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Add recieved message to our messageList for display purposes
                        messageList.Add("Received from client " + id.ToString() + ": " + data.ToString());

                        //Save the last time we recieved a message
                        clientObj.SetLastMessageTime(DateTime.UtcNow);

                        //Used for heart beat to identify if a client started transmitting data
                        clientObj.SetConnectionStarted(true);


                        // Process the data sent by the client
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response (for test only)
                        stream.Write(msg, 0, msg.Length);
                        messageList.Add("Sent to client " + id.ToString() + ": " + data.ToString());

                    }

                }
            }
            //}
        }

        public void DisconnectClient(String ip)
        {
            ClientTcp client = clientsList.First(el => el.GetIp() == ip);
            lock(client.clientAliveLock)
            {
                client.StopConnection();
                clientsList.Remove(client);
                client.getListener().Close();
            }
            messageList.Add("Client " + client.getId().ToString() + " disconnected!");
        }

        public void SendStringToClient(String ip, String message)
        {
            ClientTcp client = clientsList.First(el => el.GetIp() == ip);
            byte[] messageByte = Encoding.ASCII.GetBytes(message);
            client.GetStream().Write(messageByte, 0, messageByte.Length);
        }

        private void CheckConnections()
        {
            while(true)
            {
                Thread.Sleep(50);
                foreach(ClientTcp el in clientsList)
                {
                    lock(el.clientAliveLock)
                    { 
                        TcpClient client = el.getListener();
                        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                        TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(client.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint)).ToArray();

                        if (tcpConnections != null && tcpConnections.Length > 0)
                        {
                            TcpState stateOfConnection = tcpConnections.First().State;
                            if (stateOfConnection == TcpState.Established)
                            {
                                // Connection is OK
                            }
                            else
                            {
                                el.StopConnection();
                                clientsList.Remove(el);
                                client.Close();
                                messageList.Add("Client " + el.getId().ToString() + " disconnected!");
                                break;
                            }

                        }
                    }

                }
            }

            
        }

        private void ClientHeartbeat(object timeObj) // Used to check if a client failed to send us a "Heart beat" signal
        {
            double time = (int) timeObj;
            while(true)
            {
                DateTime timeNow = DateTime.UtcNow;
                TimeSpan timeDifference;
                try
                {
                    foreach (ClientTcp client in clientsList)
                    {
                        lock (client.clientAliveLock)
                        {
                            if (client.GetConnectionStarted() == true)
                            {
                                timeDifference = timeNow - client.GetLastMessageTime();
                                if (timeDifference.TotalSeconds > time)
                                {
                                    client.StopConnection();
                                    clientsList.Remove(client);
                                    messageList.Add("Client " + client.getId().ToString() + " failed to respond!");
                                    break;
                                }
                            }
                        }

                    }
                }
                catch
                {

                }
               
            }
        }

        public string getMessage()
        {
            string message;
            if (messageList.Count() > 0)
            {
                message = this.messageList[0];
                messageList.RemoveAt(0);
            }
            else
            {
                message = null;
            }
            return message;
        }

        public List<string> GetClientsIps()
        {
            List<string> ret = new List<string>();
            foreach(ClientTcp client in clientsList)
            {
                ret.Add(client.GetIp());
            }
            return ret;
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();
            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();
                string cliIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                NetworkStream clientStream = client.GetStream();
                clientStream.Write(Encoding.ASCII.GetBytes("Salut"), 0, Encoding.ASCII.GetBytes("Salut").Length);
                clientStream.Flush();
            }
        }

    }

   

}
