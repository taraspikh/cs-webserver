// -----------------------------------------------------------------------
// <copyright file="Handler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebServer.BusinessLogic.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Abstract handler class. Handles requests and returns responces.
    /// </summary>
    public abstract class Handler
    {
        /// <summary>
        /// Gets or sets the next handler.
        /// </summary>
        public Handler NextHandler { get; protected set; }

        /// <summary>
        /// Tries to handle request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The WebServer.BusinessLogic.Response.
        /// </returns>
        public abstract Response Handle(Request request);

        /// <summary>
        /// Sets next handler.
        /// </summary>
        /// <param name="handler">
        /// Next handler.
        /// </param>
        public void SetNext(Handler handler)
        {
            this.NextHandler = handler;
        }

        /// <summary>
        /// Identifies content type of the request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// String.
        /// </returns>
        protected static string IdentifyContentType(Request request)
        {
            string type = string.Empty;
            Regex regex = new Regex(@"\.[a-z]*$"); // Filename extension from path.
            switch (regex.Match(request.HttpPath).Value)
            {
                case ".html":
                    type = "text/html";
                    break;
                case ".htm":
                    type = "text/html";
                    break;
                case ".css":
                    type = "text/css";
                    break;
                case ".js":
                    type = "text/javascript";
                    break;
                case ".png":
                    type = "image/png";
                    break;
                case ".jpg":
                    type = "image/jpeg";
                    break;
                case ".jpeg":
                    type = "image/jpeg";
                    break;
                case ".gif":
                    type = "imane/gif";
                    break;
            }
            return type;
        }

        /// <summary>
        /// Compresses byte array to new byte array.
        /// </summary>
        public static byte[] GzipCompress(byte[] raw)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memStream, CompressionMode.Compress))
                {
                    gzip.Write(raw, 0, raw.Length);
                }

                return memStream.ToArray();
            }
        }

        /// <summary>
        /// Converts text to byte array.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The System.Byte[].
        /// </returns>
        public byte[] TextToByteArray(string text, Encoding encoding)
        {
            return encoding.GetBytes(text);
        }
    }
}
