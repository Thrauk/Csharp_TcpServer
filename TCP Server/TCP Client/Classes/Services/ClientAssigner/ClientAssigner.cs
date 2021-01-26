using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.Client.Classes;
using TCP_Server.TCP_Client.Classes.Clients;

namespace TCP_Server.TCP_Client.Classes.Services.ClientAssigner
{
    class ClientAssigner
    {
        private static List<string> clientTypes = new List<string> { "ARDUINO", "PHONE" };

        public static bool TypeVerify(string identityMessage)
        {
            if(clientTypes.Contains(identityMessage))
            {
                return true;
            }
            return false;
        }

        public static Client TypeAssigner(string identity_message, Client BaseClient)
        {
            if (identity_message == "ARDUINO")
            {
                return new ArduinoClient(BaseClient.GetClientListener());
            }
            //SHOULD NEVER REACH THIS POINT
            return new UnknownClient(BaseClient.GetClientListener());
        }
    }
}
