// -----------------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebServer.BusinessLogic.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WebServer.BusinessLogic.Logs;

    /// <summary>
    /// Handles errors.
    /// </summary>
    public class ErrorHandler : Handler
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
            Response toReturn = new Response();



            toReturn.MimeType = "text/html";
            toReturn.HttpResponseCode = 404;
            toReturn.HttpResponseCodeDescription = "Not Found";
            toReturn.Data = this.TextToByteArray(this.CreateNotFoundWebpage(), Encoding.UTF8);
            
            // Log error
            Logger.Instance.Log(string.Format("Error {0} {1} on path {2}", toReturn.HttpResponseCode, toReturn.HttpResponseCodeDescription, request.HttpPath));

            return toReturn;
        }

        public string CreateNotFoundWebpage()
        {
            // TODO: Make anchors.
            string webPage = "<html>\n<head>\n<title>Directory\n</title>\n</head>\n"
                             + "<body>\n<h1>404 Not Found</h1></body>\n</html>";

            return webPage;
        }
    }
}
