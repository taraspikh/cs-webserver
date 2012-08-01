using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using WebServer.BusinessLogic.Helpers;
using WebServer.BusinessLogic.Logs;
using WebServer.BusinessLogic;

namespace WebServer.Tests
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void Logger_saves_data()
        {
            string path = Logger.Instance.PathToLogFile;
            Logger.Instance.Log("Test");

            string text = File.ReadAllText(path);
            Assert.IsTrue(text.Contains("Test"));

            //check if Logger file contains correct 
        }

        [TestMethod]
        public void Logger_saves_date_stamp()
        {
            string path = Logger.Instance.PathToLogFile;
            var timeSaved = DateTime.Now;
            Logger.Instance.Log("Test");
            

            string text = File.ReadAllText(path);
            string timeCompare = string.Format("{0:00}:{1:00}:{2:00}", timeSaved.Hour, timeSaved.Minute,
                                               timeSaved.Second);
            Assert.IsTrue(text.Contains(timeCompare));

            //check if Logger file contains correct 
        }

        [TestMethod]
        public void Logger_creates_file_when_it_doesnt_exist()
        {
            string path = Logger.Instance.PathToLogFile;
            Logger.Instance.Log("Test");

            bool shouldExist = File.Exists(path);
            Assert.IsTrue(shouldExist);
        }

        [TestMethod]
        public void Logger_creates_file_on_date_change()
        {
            Logger.Instance.Log("TestMessage1");
            string path1 = Logger.Instance.PathToLogFile;

            Logger.Instance.Clock = new FakeClock(new DateTime(2013, 07, 26));
            Logger.Instance.Log("TestMessage2");
            string path2 = Logger.Instance.PathToLogFile;

            Logger.Instance = null;

            Assert.AreNotEqual(path1, path2);
        }

        [TestMethod]
        public void Logger_does_not_log_when_config_disabled()
        {
            //disable logging
            Configurator.Instance.EnableLogging = false;

            //log (should not log)
            Logger.Instance.Log("Logger_does_not_log_when_config_disabled");

            //enable logging
            Configurator.Instance.EnableLogging = true;

            string path = Logger.Instance.PathToLogFile;
            Trace.WriteLine(path);
            var exists = File.Exists(path);
            if (exists)
            {
                var file = File.ReadAllText(path);
                Trace.WriteLine(file);
                Assert.IsTrue(!file.Contains("Logger_does_not_log_when_config_disabled"));
                return;
            }
            
            Assert.IsFalse(exists);
        }
    }
}
