using System;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServer.BusinessLogic;
using WebServer.BusinessLogic.Web;

namespace WebServer.Tests.SessionTests
{
    [TestClass]
    public class SessionTest
    {
        [TestMethod]
        public void SessionManager_can_be_created()
        {
            Assert.IsNotNull(SessionManager.Instance);
            //int userCount = SessionManager.Instance.ConnectedUsersCount;
        }

        [TestMethod]
        public void SessionManager_has_initialized_SessionList()
        {
            Assert.IsTrue(SessionManager.Instance.SessionList != null);
        }

        [TestMethod]
        public void SessionManager_has_correct_created_date()
        {
            var createdDate = SessionManager.Instance.DateCreated;
            Assert.IsTrue(DateTime.Now >= createdDate);
        }

        Request r = new Request();
        Response resp = new Response();

        [TestMethod]
        public void Session_not_null()
        {
            var s = new Session(r);
            Assert.IsTrue(s != null);
        }

        [TestMethod]
        public void Session_has_SessionKey_on_creation()
        {
            var s = new Session(r);
            Assert.IsTrue(s.SessionKey.Length > 0);
        }

        [TestMethod]
        public void Session_is_Admin_false_by_default()
        {
            var s = new Session(r);
            Assert.IsFalse(s.IsAdmin);
        }

        [TestMethod]
        public void Session_creates_user_validation_string()
        {
            var s = Session.CreateUniqueValidation(r);
            Console.WriteLine(s);
            Assert.IsTrue(!String.IsNullOrEmpty(s));
        }

        [TestMethod]
        public void Session_creates_empty_user_validation_string_on_nulled_request()
        {
            var s = Session.CreateUniqueValidation(null);
            Console.WriteLine(s);
            Assert.IsTrue(s == String.Empty);
        }

        [TestMethod]
        public void SessionManager_can_create_session()
        {
            
            string sessionKey = SessionManager.Instance.CreateSession(r,resp).SessionKey;
            Assert.IsTrue(sessionKey != null);
        }

        [TestMethod]
        public void SessionManager_can_delete_session()
        {
            string sessionKey = SessionManager.Instance.CreateSession(r,resp).SessionKey;
            bool wasDeleted = SessionManager.Instance.DeleteSession(sessionKey,r,resp);
            Assert.IsTrue(wasDeleted);
        }

        [TestMethod]
        public void SessionManager_cant_delete_uncreated_sessions()
        {
            string sessionKey = "this_key_doesn't_exists";
            bool wasDeleted = SessionManager.Instance.DeleteSession(sessionKey, r, resp);
            Assert.IsFalse(wasDeleted);
        }

        [TestMethod]
        public void SessionManager_is_returning_Session_on_remembered_SessionKeys()
        {
            var sessionKey = SessionManager.Instance.CreateSession(r,resp).SessionKey;
            var s = SessionManager.Instance.GetSessionByKey(sessionKey,r).SessionKey;
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void SessionManager_is_returning_null_on_false_SessionKeys()
        {
            var s = SessionManager.Instance.GetSessionByKey("not_valid_SessionKey",r);
            Assert.IsNull(s);
        }

        [TestInitialize]
        public void Init()
        {
            HttpContext.Current = new HttpContext(
                    new HttpRequest("", "http://tempuri.org", ""),
                    new HttpResponse(new StringWriter())
                    );
        }
    }
}
