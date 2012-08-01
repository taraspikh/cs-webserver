using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using WebServer.BusinessLogic.Web;
using System.Text.RegularExpressions;
using WebServer.BusinessLogic.Logs;

namespace WebServer.BusinessLogic
{
    public enum EnumHttpMethod
    {
        /// <summary>
        /// for GET Metod
        /// </summary>
        GET,
        /// <summary>
        /// for POST Metod
        /// </summary>
        POST,
        /// <summary>
        /// for something else
        /// </summary>
        UNKNOWN,
    }

    public enum EnumAcceptEncoding
    {
        /// <summary>
        /// Doesn't accept compression
        /// </summary>
        None,
        /// <summary>
        /// Accepts Gzip compression
        /// </summary>
        Gzip
    }

    public class Request
    {

        /// <summary>
        /// Keep metod of HTTP request
        /// </summary>
        private EnumHttpMethod _httpMethod;
        /// <summary>
        /// Keep version of HTTP request
        /// </summary>
        private string httpVersion;
        /// <summary>
        /// Keep path from HTTP request
        /// </summary>
        private string httpPath;
        /// <summary>
        /// Keep all request text
        /// </summary>
        private string _rawData;
        /// <summary>
        /// Keep information about Range
        /// </summary>
        private long rangeStart;
        /// <summary>
        /// Keep host 
        /// </summary>
        private string host;
        /// <summary>
        /// Keep User-Agent
        /// </summary>
        private string userAgent;
        /// <summary>
        /// Keep Accept-Encoding
        /// </summary>
        private EnumAcceptEncoding acceptEncoding = EnumAcceptEncoding.None;
        /// <summary>
        /// Keep information about cookies
        /// </summary>
        private string cookiesRaw;
        /// <summary>
        /// Keep information about having cookies in request
        /// </summary>
        private bool hasCookie;
        /// <summary>
        /// Keep information about having Range
        /// </summary>
        private bool hasRange;

        /// <summary>
        /// Keep information about cookies
        /// </summary>
        public WebsiteCookies Cookies { get; set; }

        /// <summary>
        /// Property for keeping metod of HTTP request
        /// <Gets>get HttpMethod</Gets>
        /// <sets>set HttpMethod</sets>
        /// </summary>
        public EnumHttpMethod HttpMethod
        {
            get { return this._httpMethod; }
        }

        /// <summary>
        /// Property for keeping version of HTTP request
        /// <Gets>httpVersion</Gets>
        /// <sets>httpVersion</sets>
        /// </summary>
        public string HttpVersion
        {
            get { return this.httpVersion; }
        }

        /// <summary>
        /// RawData
        /// <Gets>RawData</Gets>
        /// </summary>
        public string RawData
        {
            get { return this._rawData; }

        }

        /// <summary>
        /// Information about Range
        /// <Gets>a number of RangeStart</Gets>
        /// </summary>
        public long RangeStart
        {
            get { return this.rangeStart; }

        }

        /// <summary>
        /// Keep Host 
        /// <Gets>host</Gets>
        /// </summary>
        public string Host
        {
            get { return this.host; }
            set { this.host = value; }
        }

        /// <summary>
        /// Keep User-Agent
        /// <Gets>User agent</Gets>
        /// </summary>
        public string UserAgent
        {
            get { return this.userAgent; }
            set { this.userAgent = value; }
        }


        /// <summary>
        /// Keep Accept-Encoding
        /// <Gets>EnumAcceptEncoding for compressions like None or Gzip</Gets>
        /// </summary>
        public EnumAcceptEncoding AcceptEncoding
        {
            get { return this.acceptEncoding; }
        }

        /// <summary>
        /// Gets or sets the http path.
        /// </summary>
        public string HttpPath
        {
            get { return this.httpPath; }
            set { this.httpPath = value; }
        }

        /// <summary>
        /// Cookies String
        /// <Gets>string with information about cookies</Gets>
        /// </summary>
        public string CookiesRaw
        {
            get { return this.cookiesRaw; }

        }

        /// <summary>
        /// Cheak having field Range
        /// <gets>is field Range in request</gets>
        /// </summary>
        public bool HasRange
        {
            get { return this.hasRange; }

        }

        /// <summary>
        /// Cheak having Cookies
        /// <gets>is field Cookie in request</gets>
        /// </summary>
        public bool HasCookie
        {
            get { return this.hasCookie; }

        }

        /// <summary>
        /// Constructor of Request class
        /// </summary>
        public Request()
        {
            this._httpMethod = EnumHttpMethod.UNKNOWN;
            this.httpVersion = "Unknown";
            hasRange = false;
            hasCookie = false;
        }

        /// <summary>
        /// Main Metod for parsing request
        /// </summary>
        /// <param name="data">Array of HTTP request</param>
        public void ParseRequest(byte[] data)
        {
            // VK: set correct request
            IsCorrect = true;

            // Make all request ToString()
            string requestLine = Encoding.ASCII.GetString(data);
            // VK: fix \0\0\0\0\0\0\0 request line by checking for existance of "\r\n":
            int iCarriageReturn = requestLine.IndexOf("\r\n");
            if (iCarriageReturn < 0)
            {
                // log error in request
                Logger.Instance.Log("No CR+LF in request, so header is not present. Also request is not in correct format.");
                
                // set incorrect request
                IsCorrect = false;

                // stop processing it by returning the method
                return;
            }

            // Store header line
            string headLine = requestLine.Substring(0, iCarriageReturn);

            // VK: use Trace instead of Console, prevents from writing to the response stream?
            //Trace.Write(requestLine);

            // Parsing information
            this.ParseStartingLine(headLine);
            this.ParseCookies(requestLine);
            this.ParseHost(requestLine);
            this.ParseUserAgent(requestLine);
            this.ParseAcceptEncoding(requestLine);
            this.FillRawData(requestLine);
            this.FillRange(requestLine);
        }

        /// <summary>
        /// Sets if 
        /// </summary>
        /// <author>VK</author>
        public bool IsCorrect { get; private set; }

        /// <summary>
        /// SessionKey variable for user verification. Can be null when not logged in
        /// </summary>
        public string SessionKey { get; set; }

        /// <summary>
        /// IP Address of remote user, for Session creation
        /// </summary>
        public string UserIpAddress { get; set; }

        /// <summary>
        /// Accept HTTP header line.
        /// Processing and filling 
        /// </summary>
        /// <param name="startingLine">text of HTTP header line</param>
        private void ParseStartingLine(string startingLine)
        {
            // Parsing HTTPversion
            if (startingLine.StartsWith("GET"))
            {
                _httpMethod = EnumHttpMethod.GET;
            }
            else
            {
                if (startingLine.StartsWith("POST"))
                {
                    _httpMethod = EnumHttpMethod.POST;
                }
                else
                {
                    _httpMethod = EnumHttpMethod.UNKNOWN;
                }
            }
            // Console.WriteLine(HttpMethod);
            // Parsing HTML version
            httpVersion = Helpers.WebserverConstants.Unknown;
            if (startingLine.Contains(Helpers.WebserverConstants.HttpVersion11))
            {
                httpVersion = Helpers.WebserverConstants.HttpVersion11;

            }
            else if (startingLine.Contains(Helpers.WebserverConstants.HttpVersion10))
            {
                httpVersion = Helpers.WebserverConstants.HttpVersion10;

            }
            // Console.WriteLine(httpVersion);

            // Parsing path
            string pathLine = startingLine.Substring(startingLine.IndexOf("/"));
            httpPath = pathLine.Substring(0, pathLine.IndexOf(" "));
            // Console.WriteLine(httpPath);

        }

        /// <summary>
        /// Parsing Host in request
        /// </summary>
        /// <param name="message">request string</param>
        private void ParseHost(string message)
        {

            String stringToParse = "Host";
            if (message.Contains(stringToParse))
            {

                String hostString = message.Substring(message.LastIndexOf(stringToParse));
                hostString = hostString.Substring(stringToParse.Length + 1, hostString.IndexOf("\r\n"));

                if (hostString.Contains("\r\n"))

                    host = hostString.Remove(hostString.IndexOf("\r\n")).Trim();

                else

                    host = hostString.Trim();

            }
            // Console.WriteLine(Host);

        }

        private Regex _regexUserAgent = new Regex(@"User\-Agent:\s*(.*?)\s*$", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Parsing Host in request
        /// </summary>
        /// <param name="message">request string</param>
        private void ParseUserAgent(string message)
        {
            //Regex.Compiled is 4 times faster than IndexOf
            var match = _regexUserAgent.Match(message);
            if (match.Success)
            {
                userAgent = match.Groups[1].Value;
            }
        }

        private Regex _regexAcceptEncoding = new Regex(@"Accept\-Encoding:\s*(.*?)\s*$", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Parsing Accept-Encoding in request
        /// </summary>
        /// <param name="message">request string</param>
        private void ParseAcceptEncoding(string message)
        {
            var match = _regexAcceptEncoding.Match(message);
            if (match.Success)
            {
                if (match.Groups[1].Value.Contains("gzip"))
                {
                    acceptEncoding = EnumAcceptEncoding.Gzip;
                }else
                {
                    acceptEncoding = EnumAcceptEncoding.None;
                }
            }
        }

        /// <summary>
        /// Find cookies in request and process
        /// </summary>
        /// <param name="message">request string</param>
        private void ParseCookies(string message)
        {

            string wordToParse = "Cookie";
            if (message.Contains(wordToParse))
            {

                this.hasCookie = true;
                string hostString = message.Substring(message.LastIndexOf(wordToParse));
                hostString = hostString.Substring(wordToParse.Length + 1, hostString.IndexOf("\r\n"));

                if (hostString.Contains("\r\n"))
                    cookiesRaw = hostString.Remove(hostString.IndexOf("\r\n")).Trim();
                else
                    cookiesRaw = hostString.Trim();

                //parse into cookies
                if (!Configurator.Instance.IsCookielessSession)
                {
                    Cookies = new WebsiteCookies();
                    var cookies = cookiesRaw.Split(new[] {';'});
                    foreach (var cooky in cookies)
                    {
                        var cookyKeyValue = cooky.Split(new[] {'='});
                        if (cookyKeyValue.Length == 2)
                        {
                            string key = cookyKeyValue[0].Trim();
                            string value = cookyKeyValue[1].Trim();

                            Cookies.Add(key, value);
                        }
                    }
                }

            }
            // Console.WriteLine(CookiesRaw);

        }

        /// <summary>
        /// Filling all request in RawData
        /// </summary>
        /// <param name="message"></param>
        private void FillRawData(string message)
        {
            _rawData = message;
        }

        /// <summary>
        /// Trying to find field "Range" in request
        /// If found - fill 
        /// </summary>
        /// <param name="message"></param>
        private void FillRange(string message)
        {
            if (message.Contains("Range"))
            {
                hasRange = true;
                string rangeString = message.Substring(message.IndexOf("Range"));
                rangeString = rangeString.Substring(0, rangeString.IndexOf("\r\n"));
                //Make line like "Range: bytes=88080384-
                rangeString = rangeString.Substring(rangeString.IndexOf("=") + 1);
                rangeString = rangeString.Substring(0, rangeString.IndexOf("-"));
                try
                {
                    rangeStart = Convert.ToInt64(rangeString);
                }
                catch (Exception)
                {
                    rangeStart = 0;
                    this.hasRange = false;
                }

            }
        }




    }
}
