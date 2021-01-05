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
                if (messageList.Count() > 0)
                {
                    foreach (string message in messageList)
                    {
                        messageDisplay.AppendText(message);
                    }
                }
            }

        }

    }
}
