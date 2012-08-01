using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServer.BusinessLogic.Web;

namespace WebServer.Tests.SessionTests
{
    [TestClass]
    public class WebsiteCookiesTest
    {
        [TestMethod]
        public void Cookie_can_be_saved_and_restored()
        {
            WebsiteCookies w = new WebsiteCookies();
            w.Add("test", "value");
            Assert.IsTrue(w.GetCookie("test").Value == "value");
        }

        [TestMethod]
        public void Cookie_can_be_overwritten()
        {
            WebsiteCookies w = new WebsiteCookies();
            w.Add("test", "value");
            w.Add("test", "newvalue");
            Assert.IsTrue(w.GetCookie("test").Value == "newvalue");
        }

        [TestMethod]
        public void Non_existant_cookie_is_nulled()
        {
            WebsiteCookies w = new WebsiteCookies();
            var value = w.GetCookie("non-existant-key");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void Cookie_can_be_removed()
        {
            WebsiteCookies w = new WebsiteCookies();
            w.Add("existant_key", "value");
            w.RequestDelete("existant_key");

            //check if cookie was set to be reset in response
            var cookie = w.GetCookie("existant_key");
            Assert.IsTrue(cookie.Value == "deleted");

            //check if cookie expiration date is lower than current date
            Assert.IsTrue(cookie.Expires < DateTime.Now);
        }

        [TestMethod]
        public void Cookie_can_be_deleted_from_collection()
        {
            WebsiteCookies w = new WebsiteCookies();
            w.Add("key", "value");
            w.Delete("key");

            Assert.IsTrue(w.Length == 0);
        }
    }
}
