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
using System.Windows.Shapes;

namespace TCP_Server
{
    /// <summary>
    /// Interaction logic for SendMenu.xaml
    /// </summary>
    public partial class SendMenu : Window
    {
        private MainWindow mainWindowReference;
        private ListView mainWindowConnectedClients;
        private Thread updateThread;
        private string transmissionMode="Unicast";
        public SendMenu()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            mainWindowReference = GetMainWindow();
            mainWindowConnectedClients = mainWindowReference.GetConnectedClientsListView();
            CopyClients();
            updateThread = new Thread((new ThreadStart(UpdateUIThread)));
            updateThread.Start();
        }

        private void UpdateUIThread()
        {
            while (true)
            {
                Thread.Sleep(250);
                Dispatcher.Invoke(() =>
                    {
                        CopyClients();
                    });
            }
        }

        public void CloseWindow()
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //CloseWindow();
            updateThread.Abort();
            mainWindowReference.RemoveSendMenuWindow();
            Thread.Sleep(25);
        }

        private void CopyClients()
        {
            if (mainWindowReference.GetConnectedClientsListView().Items.Count != connectedClients.Items.Count)
            {
                connectedClients.Items.Clear();
                if (mainWindowReference != null)
                {
                    foreach (string item in mainWindowReference.GetConnectedClientsListView().Items)
                    {
                        connectedClients.Items.Add(item);
                    }
                }
            }

        }

        private MainWindow GetMainWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    return window as MainWindow;
                }
            }
            return null;
        }


        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void Unicast_Checked(object sender, RoutedEventArgs e)
        {
            connectedClients.IsEnabled = true;
            connectedClients.SelectionMode = SelectionMode.Single;
            transmissionMode = "Unicast";
        }

        private void MulticastButton_Checked(object sender, RoutedEventArgs e)
        {
            connectedClients.IsEnabled = true;
            connectedClients.SelectionMode = SelectionMode.Multiple;
            transmissionMode = "Multicast";
        }

        private void BroadcastButton_Checked(object sender, RoutedEventArgs e)
        {
            connectedClients.UnselectAll();
            connectedClients.IsEnabled = false;
            transmissionMode = "Broadcast";
        }

        private void UnicastSend(string message)
        {
            mainWindowReference.SendMessageToClientIp(connectedClients.SelectedItem.ToString(), message);
            //mainWindowReference.AddToMessageList(message);
        }

        private void MulticastSend(string message)
        {
            foreach (string ip in connectedClients.SelectedItems)
            {
                mainWindowReference.SendMessageToClientIp(ip, message);
                //mainWindowReference.AddToMessageList(message);
            }
        }

        private void BroadcastSend(string message)
        {
            foreach (string ip in connectedClients.Items)
            {
                mainWindowReference.SendMessageToClientIp(ip, message);
                //mainWindowReference.AddToMessageList(message);
            }
        }

        private void SendMsgButton_Click(object sender, RoutedEventArgs e)
        {
            string message = textBox.Text;
            if (connectedClients.Items.Count != 0)
            {
                if (message.Length > 0)
                {
                    if (transmissionMode.Equals("Broadcast"))
                    {
                        BroadcastSend(message);
                    }
                    else if (connectedClients.SelectedItems.Count != 0)
                    {
                        if (transmissionMode.Equals("Unicast"))
                        {
                            UnicastSend(message);
                        }
                        else if (transmissionMode.Equals("Multicast"))
                        {
                            MulticastSend(message);
                        }
                    }
                    else
                        MessageBox.Show("No client selected", "Error");
                }
                else
                    MessageBox.Show("Message to send cannot be empty!", "Error");
            }
            else
            {
                MessageBox.Show("No client connected", "Error");
            }

        }
    }
}
