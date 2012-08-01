using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebServer.BusinessLogic.Web;
using System.Collections.ObjectModel;

namespace WebServer.BusinessLogic
{
    public class HeadersCollection : Collection<string>
    {
        public HeadersCollection() : base (new List<string>())
        {
        }

        public bool Exists (Predicate<string> match)
        {
            var items = (List<string>) this.Items;
            return items.Exists(match);
        }
    }

    public class ResponseHeaderCreator
    {
        /*
         * Response should look like this
         * 
        HTTP/1.1 200 OK
        Date: Sun, 09 Mar 2008 16:05:07 GMT
        Connection: close
        Server: MyServer/1.0
        Accept-Ranges: bytes
        Content-Type: text/html
        Content-Length: 170
        Last-Modified: Sun, 09 Mar 2008 16:05:07 GMT
         */

        private HeadersCollection _headersList;
        private Response _response;

        public ResponseHeaderCreator(Response response)
        {
            _headersList = new HeadersCollection();
            _response = response;
        }

        /// <summary>
        /// Creates list of parameters for the Header
        /// </summary>
        /// <returns></returns>
        public HeadersCollection Create()
        {
            //add Header with status and description
            _headersList.Add(string.Format("{0} {1} {2}", _response.HttpVersion, _response.HttpResponseCode, _response.HttpResponseCodeDescription)); // HTTP/1.0 200 OK
            
            //add current Date
            _headersList.Add(String.Format(CultureInfo.InvariantCulture, "Date: {0}", DateTime.Now.ToString("r", CultureInfo.InvariantCulture)));

            //add Connection if needed
            if (!String.IsNullOrEmpty(_response.ConnectionStatus))
            {
                _headersList.Add(string.Format("Connection: {0}", _response.ConnectionStatus));
            }

            //add Location if needed
            if (!String.IsNullOrEmpty(_response.Location))
            {
                _headersList.Add(string.Format("Location: {0}", _response.Location));
            }

            //add Server name and version
            _headersList.Add(string.Format("Server: {0}/{1}", Configurator.Instance.ServerName, Configurator.Instance.Version));

            //add Accept ranges if needed
            if (_response.AcceptsRanges)
            {
                _headersList.Add(string.Format("Accept-Ranges: {0}", _response.AcceptsRanges ? "bytes" : "none"));
            }

            //add Mime Type
            if (!String.IsNullOrEmpty(_response.MimeType))
            {
                _headersList.Add(string.Format("Content-Type: {0}", _response.MimeType));
            }

            //add length of Data
            if (_response.Data != null)
            {
                _headersList.Add(string.Format("Content-Length: {0}", _response.Data.Length));
            }

            if(_response.ContentEncoding != null)
            {
                _headersList.Add(string.Format("Content-Encoding: {0}", _response.ContentEncoding));
            }

            //add last modified date of file, if Handler did set this value
            if (_response.LastModified > DateTime.MinValue)
            {
                _headersList.Add(string.Format(CultureInfo.InvariantCulture, "Last-Modified: {0}", String.Format("{0:r}", _response.LastModified)));
            }

            //add set-cookies
            if (_response.Cookies.Length > 0)
            {
                foreach (Cookie cookie in _response.Cookies.Items)
                {
                    string pathStr = String.Empty;
                    if (cookie.Path != null)
                    {
                        pathStr = string.Format("; Path={0}", cookie.Path);
                    }

                    string expiresStr = String.Empty;
                    if (cookie.Expires.HasValue)
                    {
                        expiresStr = string.Format(CultureInfo.InvariantCulture, "; Expires={0:r}", cookie.Expires.Value);
                    }
                    _headersList.Add(string.Format("Set-Cookie: {0}={1}{2}{3}", cookie.Key, cookie.Value, pathStr, expiresStr));
                }
                
            }


            return _headersList;
        }

        /// <summary>
        /// Override ToString method to return proper format for HEADER
        /// </summary>
        public override string ToString()
        {
            const string separator = "\r\n";
            return String.Join(separator, _headersList.ToArray()) + separator;
        }
    }
}