using WebServer.BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebServer.Tests.ResponseTests
{
    /// <summary>
    ///This is a test class for ResponseTest and is intended
    ///to contain all ResponseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResponseTest
    {
      
        /// <summary>
        ///A test for Response Constructor
        ///</summary>
        [TestMethod]
        public void ResponseConstructorTest()
        {
            Response target = new Response();
            Assert.IsTrue(target != null, "Response can't be created");
        }

        /// <summary>
        ///A test for Data
        ///</summary>
        [TestMethod]
        public void Data_is_null_on_creation_of_Response()
        {
            Response target = new Response();
            Assert.AreEqual(target.Data, null);
        }

        /// <summary>
        ///A test for HttpVersion
        ///</summary>
        [TestMethod]
        public void HTTPVersion_is_1_0_by_default()
        {
            Response target = new Response();
            const string version = "HTTP/1.0";
            Assert.AreEqual(target.HttpVersion, version);
        }

        
        [TestMethod]
        public void Http_code_status_is_200_by_default()
        {
            Response target = new Response();
            const int data = 200;
            Assert.AreEqual(target.HttpResponseCode, data);
        }

        [TestMethod]
        public void Http_code_description_is_OK_by_default()
        {
            Response target = new Response();
            const string data = "OK";
            Assert.AreEqual(target.HttpResponseCodeDescription, data);
        }
    }
}