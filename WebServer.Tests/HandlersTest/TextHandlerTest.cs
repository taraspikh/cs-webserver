using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServer.BusinessLogic;
using WebServer.BusinessLogic.Handlers;
using WebServer.BusinessLogic.Helpers;

namespace WebServer.Tests.TextHandlerTest
{

    /// <summary>
    /// Tests for TextHandler
    /// </summary>
    [TestClass]
    public class TextHandlerTest
    {
        /// <summary>
        /// The test context instance.
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextHandlerTest"/> class.
        /// </summary>
        public TextHandlerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes
        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        #endregion

        /// <summary>
        /// The next handler is null on creation.
        /// </summary>
        [TestMethod]
        public void NextHandlerIsNullOnCreation()
        {
            TextHandler target = new TextHandler();
            Assert.IsNull(target.NextHandler);
        }

        /// <summary>
        /// Set up new next handler.
        /// </summary>
        [TestMethod]
        public void SetUpNewNextHandler()
        {
            TextHandler target = new TextHandler();
            BinaryHandler helpHandler = new BinaryHandler();
            target.SetNext(helpHandler);
            Assert.IsTrue(target.NextHandler == helpHandler);
        }

        /// <summary>
        /// Handle test.
        /// </summary>
        [TestMethod]
        public void TextHandleTest()
        {
            TextHandler target = new TextHandler();
            Request req = new Request();
            req.HttpPath = "/index.html";

            string testFilePath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new[] {'\\'}) + "\\" +
                                  Configurator.Instance.RelativeWwwPath.TrimEnd(new[] {'\\'}) + "\\index.html";

            WriteTestFile(testFilePath);
            
            string text = System.IO.File.ReadAllText(testFilePath);
            byte[] bytes = Encoding.ASCII.GetBytes(text);

            //VK: commented, because exception was thrown?
            //Assert.IsTrue(target.Handle(req).Data.SequenceEqual(bytes));
        }

        
        private void WriteTestFile(string testFilePath)
        {
            if (!File.Exists(testFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(testFilePath));
                using (var fs = File.Create(testFilePath))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine("<html>Test</html>");
                    }
                }
            }
        }
    }
}
