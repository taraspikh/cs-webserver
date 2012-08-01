using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServer.BusinessLogic;

namespace WebServer.Tests.ResponseTests
{
    [TestClass]
    public class ResponseBuilderTest
    {
        //create sample data for Response
        private byte[] data = Encoding.UTF8.GetBytes("This is just a test data!");

        [TestMethod]
        public void ResponseBuilder_can_be_created()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            var rb = new ResponseBuilder(r);
            Assert.IsTrue(rb != null, "ResponseBuilder was not created properly");
        }

        [TestMethod]
        public void ResponseBuilder_returns_500_error_on_empty_data()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            var rb = new ResponseBuilder(r);
            rb.Response.Data = null;
            rb.GetResponseBytes();
            Assert.AreEqual(500, rb.Response.HttpResponseCode, "On nulled data response didn't have 500 Error");
        }

    }
}
