using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServer.BusinessLogic;

namespace WebServer.Tests.HandlersTest
{
    using WebServer.BusinessLogic.Handlers;

    /// <summary>
    /// Summary description for DirectoryHandlerTest
    /// </summary>
    [TestClass]
    public class DirectoryHandlerTest
    {
        public DirectoryHandlerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CreatesProperWebPage()
        {
            DirectoryHandler target = new DirectoryHandler();
            Request req = new Request();
            req.HttpPath = "/misc/";
            Assert.AreEqual("<html><head><title>Directory</title></head><body></body></html>", target.Handle(req));
            // TODO: Finish test.
        }
    }
}
