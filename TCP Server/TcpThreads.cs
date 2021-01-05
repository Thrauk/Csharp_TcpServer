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
    partial class TcpServer
    {
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
                catch (SocketException)
                {
                    return;
                }
                lock (listLock)
                {
                    // adaug clientul in lista de clienti conectati
                    //clientsList.Add(client);
                    clientsQueue.Enqueue(client);
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

            try
            {
                stream = client.GetStream();
                clientObj.SetNetworkStream(stream);
            }
            catch
            {
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
                        //clientsList.Remove(clientObj);
                        clientsQueue = new Queue<ClientTcp>(clientsQueue.Where(element => element != clientObj));
                        client.Close();
                        messageList.Add("Client " + clientObj.getId().ToString() + " disconnected!");
                    }
                    if (i == 0)
                    {
                        clientObj.StopConnection();
                        //clientsList.Remove(clientObj);
                        clientsQueue = new Queue<ClientTcp>(clientsQueue.Where(element => element != clientObj));
                        client.Close();
                        messageList.Add("Client " + clientObj.getId().ToString() + " disconnected!");
                    }
                    else
                    {


                        //Decode recieved message from bytes to string
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Add recieved message to our messageList for display purposes
                        messageList.Add("Received from client " + id.ToString() + ": " + data.ToString());


                        if (commands.Contains(data))
                        {
                            messageCommandList.Enqueue(data);
                        }

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

        private void ResponseThread()
        {
            byte[] message;
            while (true)
            {
                if (messageCommandList.Count > 0)
                {
                    if (demoDevices.Count == 0)
                    {
                        messageCommandList.Dequeue();
                    }
                    else
                    {
                        try
                        {
                            foreach (ClientTcp client in demoDevices)
                            {
                                message = System.Text.Encoding.ASCII.GetBytes(messageCommandList.Dequeue());
                                client.GetStream().Write(message, 0, message.Length);
                            }
                        }
                        catch (Exception) { }
                    }
                }
            }

        }

        private void CheckConnections()
        {
            TcpClient client;
            IPGlobalProperties ipProperties;
            TcpConnectionInformation[] tcpConnections;
            while (true)
            {
                Thread.Sleep(50);
                //foreach(ClientTcp el in clientsList)
                foreach (ClientTcp el in clientsQueue)
                {
                    lock (el.clientAliveLock)
                    {
                        try
                        {
                            client = el.getListener();
                            ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                            tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(client.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint)).ToArray();
                        }
                        catch //Client had an error or disconnected
                        {
                            break;
                        }

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
                                //clientsList.Remove(el);
                                clientsQueue = new Queue<ClientTcp>(clientsQueue.Where(element => element != el));
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
            double time = (int)timeObj;
            while (true)
            {
                DateTime timeNow = DateTime.UtcNow;
                TimeSpan timeDifference;
                try
                {
                    //foreach (ClientTcp client in clientsList)
                    foreach (ClientTcp client in clientsQueue)
                    {
                        lock (client.clientAliveLock)
                        {
                            if (client.GetConnectionStarted() == true)
                            {
                                timeDifference = timeNow - client.GetLastMessageTime();
                                if (timeDifference.TotalSeconds > time)
                                {
                                    client.StopConnection();
                                    //clientsList.Remove(client);
                                    clientsQueue = new Queue<ClientTcp>(clientsQueue.Where(element => element != client));
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
    }


}
