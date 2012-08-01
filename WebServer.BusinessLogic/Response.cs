// Response
// created by VK

using System;
using WebServer.BusinessLogic.Web;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Response is responsible to serve data needed for RequestBuilder and Communicator to create a reply of our webserver for internet browser
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Response()
        {
            Data = null;
            HttpVersion = Helpers.WebserverConstants.HttpVersion10;
            HttpResponseCode = 200;
            HttpResponseCodeDescription = "OK";
            Cookies = new WebsiteCookies();
        }

        /// <summary>
        /// Constructor that should initialiaze all values that can have a setter
        /// </summary>
        /// <param name="mimeType">MimeType of the data</param>
        /// <param name="httpVersion">Version, HTTP/1.0 or HTTP/1.1</param>
        /// <param name="data">Byte array of data to respond with in body</param>
        public Response(string mimeType, string httpVersion, byte[] data) : this()
        {
            MimeType = mimeType;
            HttpVersion = httpVersion;
            Data = data;
        }

        /// <summary>
        /// Bytes of the Data that are needed to be responded with
        /// </summary>
        public byte[] Data { get; /*private*/ set; }

        /// <summary>
        /// Mime type of file, for example: "text/html" or "image/gif"
        /// </summary>
        public string MimeType { get; /*private*/ set; }

        /// <summary>
        /// Version of the HTTP protocol. Either HTTP/1.0 OR HTTP/1.1
        /// </summary>
        public string HttpVersion { get; /*private*/ set; }

        /// <summary>
        /// Hostname, for example: www.google.com
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Bytes of the data webserver is returning to the client, start position
        /// </summary>
        public long RangeStart { get; set; }

        /// <summary>
        /// HTTP Response code. Default "200"
        /// </summary>
        public int HttpResponseCode { get; set; }

        /// <summary>
        /// HTTP Response description for the code. Default "OK"
        /// </summary>
        public string HttpResponseCodeDescription { get; set; }

        /// <summary>
        /// Connection status. Can be "close" or empty
        /// </summary>
        public string ConnectionStatus { get; set; }

        /// <summary>
        /// Last modified date of file. Handler should set this value
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Indicates if Response Accepts ranges. true will set Accept-Range header to "bytes", false to "none"
        /// </summary>
        public bool AcceptsRanges { get; set; }

        /// <summary>
        /// Encoding of responce content
        /// </summary>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// Set-Cookie cookies
        /// </summary>
        public WebsiteCookies Cookies { get; set; }

        /// <summary>
        /// If set, initiates redirect to the Location path
        /// </summary>
        public string Location { get; set; }
    }
}
