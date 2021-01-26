﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Client.Classes.Services
{
    abstract class DataSender
    {
        protected readonly NetworkStream stream;

        protected DataSender(NetworkStream stream)
        {
            this.stream = stream;
        }

        protected byte[] StringToByte(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }

        public abstract void SendMessage(byte[] message);

        public abstract void SendMessage(string message);

        public abstract void SendCommandString(string command);

        public abstract void SendCommandBytes(byte[] command);

    }
}
