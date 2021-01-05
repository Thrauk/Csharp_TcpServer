using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCP_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly object stopLock = new object();

        public delegate void rtbDelegate(string text);
        public delegate void clientDelegate(List<string> ips);

        private int connectedClientsCount;
        TcpServer myServer;
        Thread updateThread;
        SendMenu sendMenuWindow = null;

        public MainWindow()
        {
            InitializeComponent();
            this.richTextBox.Document.Blocks.Clear();
            //connectedClients.Items.Add("Test");
            //connectedClients.Items.Add("Test");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String ip = serverIp.Text;
            String port = serverPort.Text;
            myServer = new TcpServer(ip,port);
            myServer.StartServer();
            updateThread = new Thread((new ThreadStart(UpdateUIThread)));
            updateThread.Start();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            sendButton.IsEnabled = true;
            subscribeButton.IsEnabled = true;
            disconnectClientButton.IsEnabled = true;
            //myServer.StartUpdate(richTextBox);

        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            Closing_Routine();

            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            sendButton.IsEnabled = false;
            subscribeButton.IsEnabled = false;
            disconnectClientButton.IsEnabled = false;
            UpdateRichTextBox("Server closed!\r");
            //myServer.StartUpdate(richTextBox);

        }


        private void Closing_Routine()
        {
            lock (stopLock)
            {
                myServer.StopServer();
            }
            Thread.Sleep(25);
            updateThread.Abort();
            SendMenuClose();
        }



        private void UpdateUIThread()
        {
            while(true)
            {
                rtbDelegate updateRtb = new rtbDelegate(UpdateRichTextBox);
                clientDelegate updateList = new clientDelegate(AddToConnectedClient);
                Thread.Sleep(50);
                string message = myServer.getMessage();
                List<string> ipAddresses = myServer.GetClientsIps();
                if (message != null)
                {
                        Dispatcher.Invoke(() =>
                    {
                        richTextBox.AppendText(message + '\r');
                        richTextBox.ScrollToEnd();
                    }
                     );
                }
                if(ipAddresses.Count() != connectedClientsCount)
                {
                    connectedClientsCount = ipAddresses.Count();
                    Dispatcher.Invoke(() =>
                    {
                        connectedClients.Items.Clear();
                        if(ipAddresses.Count() != 0)
                            foreach (string ip in ipAddresses)
                                this.connectedClients.Items.Add(ip);
                    }
                     );
                }

            }
        }

        public void AddToConnectedClient(List<string> ips)
        {
            connectedClients.Items.Clear();
            foreach(string ip in ips)
                this.connectedClients.Items.Add(ip);
        }

        public void UpdateRichTextBox(string line)
        {
            this.richTextBox.AppendText(line);
        }

        private void Clear_Log_Button_Click(object sender, RoutedEventArgs e)
        {
            this.richTextBox.Document.Blocks.Clear();
        }

        private void DisconnectClientButton_Click(object sender, RoutedEventArgs e)
        {
            if(connectedClients.Items.Count != 0)
            {
                if(connectedClients.SelectedItems.Count == 1)
                    myServer.DisconnectClient(connectedClients.SelectedItem.ToString());
                else
                    MessageBox.Show("No client selected", "Error");
            }
            else
            {
                MessageBox.Show("No client connected", "Error");
            }
        }

        private void Send_Menu_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sendMenuWindow == null)
            {
                sendMenuWindow = new SendMenu();
                sendMenuWindow.Show();
            }
            else
            {
                MessageBox.Show("Send menu already open!", "Alert");
            }
        }

        public void RemoveSendMenuWindow()
        {
            sendMenuWindow = null;
        }

        public void SendMenuClose()
        {
            if (sendMenuWindow != null)
            {
                sendMenuWindow.CloseWindow();
                sendMenuWindow = null;
            }
        }

        public void SendMessageToClientIp(String ip, String message)
        {
            this.myServer.SendStringToClientIp(ip, message);
        }

        public void AddToMessageList(string message)
        {
            myServer.messageList.Add(message);
        }


        public ListView GetConnectedClientsListView()
        {
            return this.connectedClients;
        }

        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {  
            if (connectedClients.Items.Count != 0)
            {
                if (connectedClients.SelectedItems.Count == 1)
                    myServer.AddSubscriber(connectedClients.SelectedItem.ToString());
                else
                    MessageBox.Show("No client selected", "Error");
            }
            else
            {
                MessageBox.Show("No client connected", "Error");
            }
        }
    }
}
