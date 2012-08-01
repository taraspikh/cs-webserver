using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.BusinessLogic
{
    using System.Threading;

    public class MyHttpServer
    {
        /// <summary>
        /// Singleton for the Communicator
        /// </summary>
        private Communicator _communicator;

        public void Start()
        {
            _communicator = Communicator.Instance;
            _communicator.StartListening();
        }

        public void Stop()
        {
            _communicator.StopListening();
        }
        public void Restart()
        {
            this.Stop();
            Thread.Sleep(1000);
            this.Start();
        }
    }
}
