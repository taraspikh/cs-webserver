using System;
using System.Globalization;
using System.IO;
using System.Text;
using WebServer.BusinessLogic.Helpers;

namespace WebServer.BusinessLogic.Logs
{
    /// <summary>
    /// Has to log data into the file
    /// </summary>
    public sealed class Logger : ILogger
    {
        private static object _syncRoot = new Object();

        private IClock _clock;
        public IClock Clock
        {
            get { return _clock; }
            set { _clock = value; }
        }

        private static volatile Logger _instance;

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    // lock the thread
                    lock(_syncRoot)
                    {
                        // when locked check if was not created again by another thread
                        if (_instance == null)
                        {
                            // create new singleton instance
                            _instance = new Logger { _clock = new SystemClock() };
                            _instance._lastCreatedDate = _instance.Clock.Now;
                        }
                    }
                }
                return _instance;
            }
            set { _instance = value; }
        }

        /// <summary>
        /// Internal variable to store log file
        /// </summary>
        private string _loggerPath;

        private DateTime _lastCreatedDate;

        /// <summary>
        /// Saves data into the file
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            // do not log when logging is not enabled
            if (!Configurator.Instance.EnableLogging)
            {
                return;
            }

            // log otherwise, lock for multi-threading
            lock (_syncRoot)
            {
                // create necessary log file
                CreateLogFileIfNeeded();

                // write custom message with default HH:mm:ss prefix
                using (var sw = new StreamWriter(PathToLogFile, true, Encoding.UTF8))
                {
                    string dataToWrite = String.Format(CultureInfo.InvariantCulture, "{0}\t{1}", DateTime.Now.ToString("HH:mm:ss"), message);
                    sw.WriteLine(dataToWrite);
                }
            }
        }

        /// <summary>
        /// Creates new file when it's not created yet
        /// </summary>
        private void CreateLogFileIfNeeded()
        {
            if (!File.Exists(PathToLogFile))
            {
                var fs = File.Create(PathToLogFile);
                fs.Close();
            }
        }

        /// <summary>
        /// Gets path to the log file. Changes on the day change.
        /// </summary>
        public string PathToLogFile
        {
            get
            {
                if (_lastCreatedDate.Day != Clock.Now.Day)
                {
                    //date changed, should recreate file also
                    _loggerPath = null;
                }

                if (_loggerPath == null)
                {
                    //get directory for the log files
                    string loggerDirectory = Configurator.Instance.LoggerPath;

                    //create directory if it doesn't exist
                    var dir = Directory.CreateDirectory(loggerDirectory);

                    if (dir.Exists)
                    {
                        _loggerPath = loggerDirectory.TrimEnd(new[] {'\\'}) + "\\" +
                                      String.Format(CultureInfo.InvariantCulture, "{0}.txt", Clock.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    }
                }

                return _loggerPath;
            }
        }
    }
}