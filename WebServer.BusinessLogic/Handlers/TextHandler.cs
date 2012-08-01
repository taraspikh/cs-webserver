// -----------------------------------------------------------------------
// <copyright file="TextHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebServer.BusinessLogic.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.IO;
    using WebServer.BusinessLogic.Web;

    /// <summary>
    /// Handles Text Requests
    /// </summary>
    public class TextHandler : Handler
    {
        /// <summary>
        /// Tries to handle request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The WebServer.BusinessLogic.Response.
        /// </returns>
        public override Response Handle(Request request)
        {
            if (request.HttpPath.EndsWith(".html") || request.HttpPath.EndsWith(".htm") || request.HttpPath.EndsWith(".css")
                || request.HttpPath.EndsWith(".js"))
            {
                string filePath = Configurator.Instance.RelativeWwwPath + request.HttpPath.Replace("/", "\\");

                if (!File.Exists(filePath))
                {
                    return this.NextHandler.Handle(request);
                }

                
                string text = File.ReadAllText(filePath);

                var session = SessionManager.Instance.GetCurrentSession(request);
                if (session != null && session.IsAdmin)
                {
                    text = ReplaceWithAdminPanel(text);
                }

                Byte[] data = this.TextToByteArray(text, Encoding.UTF8);
                string MIMEType = IdentifyContentType(request);
                string version = request.HttpVersion;

                Response toReturn = new Response(MIMEType, version, data);
                if (Configurator.Instance.UseResponseCompression && request.AcceptEncoding == EnumAcceptEncoding.Gzip)
                {
                    toReturn.Data = GzipCompress(toReturn.Data);
                    toReturn.ContentEncoding = "gzip";
                }

                return toReturn;


            }

            return this.NextHandler.Handle(request);
        }

        private string ReplaceWithAdminPanel(string text)
        {
            return text.Replace("<body>", "<body>\r\n[Administrator logged in. <a href=\"/admin/Logout.htm\">Logout</a>]<br />\r\n");
        }
    }
}
