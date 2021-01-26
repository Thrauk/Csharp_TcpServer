using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server
{
    interface I_ClientInterface
    {
        void StartConnection();
        void StopConnection();


        void CheckAliveCondition();
        void Identify();
        void GetClientType();

        void ExecFeature();

    }
}
