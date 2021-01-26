using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Server.TCP_Client.Classes.Threads
{
    abstract class ClientThread
    {
        protected Thread thread;
        protected abstract void ThreadBody();

        public virtual Thread GetThread()
        {
            if (thread == null)
            {
                thread = new Thread(new ThreadStart(ThreadBody));
            }
            return thread;
        }

        public virtual void StartThread()
        {
            GetThread().Start();
        }

        public virtual void AbortThread()
        {
            GetThread().Abort();
        }
    }
}
