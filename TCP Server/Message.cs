using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server
{
    class Message
    {
        string messageType;
        public string message;
        string messageToUi;

        public Message(string message)
        {
            this.message = message;
            this.messageType = MessageType(message);
            this.messageToUi = message;
        }

        public Message(string message, string messageHeader)
        {
            this.message = message;
            this.messageType = MessageType(message);
            this.messageToUi = MessageToUI(message, messageHeader);
        }

        private string MessageType(string message)
        {
            switch (message)
            {
                case "heartbeat":
                    return "heartbeat";
                case "bataie":
                    return "heartbeat";
                default:
                    return "default";
            }
        }

        private string MessageToUI(string message, string messageHeader)
        {
            return messageHeader + message;
        }

    }
}
