using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServer.BusinessLogic;
using System.Net;
using System.Threading;

namespace WebServer.Tests
{
    [TestClass]
    public class CommunicatorTest
    {
        [TestCleanup]
        public void CleanUp()
        {
            Communicator.Instance.TryToFreePortForWebserver();
        }

        [TestMethod]
        public void Instance_object_is_not_null()
        {
            Communicator c = Communicator.Instance;
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void Instance_is_not_null()
        {
            Assert.IsTrue(Communicator.Instance != null);
        }

        [TestMethod]
        public void IsActive_equals_true_while_listening()
        {
            Communicator.Instance.StartListening();
            bool isActive = Communicator.Instance.IsActive;
            Communicator.Instance.StopListening();
            
            Assert.IsTrue(isActive);
        }

        [TestMethod]
        public void IsActive_equals_false_while_not_listening()
        {
            Communicator.Instance.StartListening();
            Communicator.Instance.StopListening();
            bool isActive = Communicator.Instance.IsActive;

            Assert.IsTrue(isActive == false);
        }


        /*[TestMethod]
        public void dasd()
        {
            Communicator.Instance.StartListening();
            string address = string.Format("http://{0}:{1}/", Configurator.Instance.Host, Configurator.Instance.Port);
            Trace.WriteLine(address);
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(address);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Method = "GET";
            try
            {
                Thread.Sleep(1000);
                request.GetResponse();
            }
            catch(WebException e)
            {
                Trace.WriteLine("Ex:"+e.Message);
            }
            int connections = Communicator.Instance.ActiveConnections;
            Communicator.Instance.StopListening();
            Trace.WriteLine(connections);
            Assert.IsTrue(connections > 0);
        }*/
    }
}
