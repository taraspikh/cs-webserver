using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServer.BusinessLogic;
using System.Globalization;
using WebServer.BusinessLogic.Web;

namespace WebServer.Tests.ResponseTests
{
    [TestClass]
    public class ResponseBuilderHeaderCreatorTest
    {
        // create sample data for Response
        private byte[] data = Encoding.UTF8.GetBytes("Test!");

        [TestMethod]
        public void Is_not_null()
        {
            var r = new Response();
            var hc = new ResponseHeaderCreator(r);
            Assert.IsNotNull(hc);
        }

        [TestMethod]
        public void Returns_more_than_1_item()
        {
            
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();
            Assert.IsTrue(headers.Count > 0);
        }

        [TestMethod]
        public void Creates_heading_HTTP_version()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Exists(h =>
                h.Contains(WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10) ||
                h.Contains(WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion11)));
        }

        [TestMethod]
        public void Has_ConnectionStatus_when_it_is_set()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            r.ConnectionStatus = "close";

            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Contains("Connection: close"));
        }

        [TestMethod]
        public void Has_Accept_range_bytes()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            r.AcceptsRanges = true;

            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Contains("Accept-Ranges: bytes"), "Accept range was not set");
        }

        [TestMethod]
        public void Has_LastModified_tag()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            r.LastModified = DateTime.Now;

            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Contains("Last-Modified: " + string.Format("{0:r}", r.LastModified)));
        }

        [TestMethod]
        public void Has_ContentEncoding_tag()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            r.ContentEncoding = "gzip";

            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Contains("Content-Encoding: " + r.ContentEncoding));
        }

        [TestMethod]
        public void Has_Location_tag()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            r.Location = "/";

            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Contains("Location: " + r.Location));
        }

        [TestMethod]
        public void Has_Set_Cookie_tag()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            r.Cookies.Add("test", "cookie");

            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Contains(string.Format("Set-Cookie: {0}={1}", "test", "cookie")));
        }

        [TestMethod]
        public void Has_Set_Cookie_tag_with_Expired()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            var expirationDate = DateTime.Now.AddDays(2);
            r.Cookies.Add("test", "cookie", expirationDate);

            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Contains(string.Format(CultureInfo.InvariantCulture, "Set-Cookie: {0}={1}; Expires={2:r}", "test", "cookie", expirationDate)));
        }

        [TestMethod]
        public void Has_Set_Cookie_tag_with_Expired_and_Path()
        {
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);
            var expirationDate = DateTime.Now.AddDays(2);
            
            var c = new Cookie("test", "cookie");
            c.Expires = expirationDate;
            c.Path = "/";
            r.Cookies.Add(c);

            var hc = new ResponseHeaderCreator(r);
            var headers = hc.Create();

            Assert.IsTrue(headers.Contains(string.Format(CultureInfo.InvariantCulture, "Set-Cookie: {0}={1}; Path={2}; Expires={3:r}", "test", "cookie", c.Path, expirationDate)));
        }
    }
}
