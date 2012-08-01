// -----------------------------------------------------------------------
// <copyright file="DirectoryHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebServer.BusinessLogic.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Web;

    /// <summary>
    /// Handles directory requests.
    /// </summary>
    public class DirectoryHandler : Handler
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
            if (request.HttpPath.EndsWith("/"))
            {
                string path = Configurator.Instance.RelativeWwwPath + request.HttpPath.Replace("/", "\\");
                if(!Directory.Exists(path))
                {
                    return NextHandler.Handle(request);
                }
                DirectoryInfo directory = new DirectoryInfo(path);
                List<FileSystemInfo> descendants = new List<FileSystemInfo>();
                descendants.AddRange(directory.GetFileSystemInfos());

                if (descendants.Select(desc => desc.Name).Contains("index.html"))
                {
                    request.HttpPath += "index.html";
                    return NextHandler.Handle(request);
                }

                string webPage = this.CreateWebPage(descendants, request);

                Response toReturn = new Response("text/html", request.HttpVersion, Encoding.UTF8.GetBytes(webPage));
                if (Configurator.Instance.UseResponseCompression && request.AcceptEncoding == EnumAcceptEncoding.Gzip)
                {
                    toReturn.Data = GzipCompress(toReturn.Data);
                    toReturn.ContentEncoding = "gzip";
                }

                return toReturn;
            }

            return NextHandler.Handle(request);
        }

        /// <summary>
        /// Creates web-page showing file structure.
        /// </summary>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public string CreateWebPage(IEnumerable<FileSystemInfo> elements, Request request)
        {
            StringBuilder webPage = new StringBuilder();
            webPage.Append("<html>\n<head>\n<title>Directory\n</title>\n</head>\n");
            webPage.Append("<body>\n<table>\n<tr><th>Name</th><th>Type</th>\n</tr>");
            foreach (FileSystemInfo element in elements)
            {
                webPage.Append(string.Format("<tr><td><a href=\"{0}\">{1}</a></td><td>{2}</td></tr>",request.HttpPath+element.Name, element.Name, element.Extension));
            }
            webPage.Append("</table>\n</body>\n</html>");
            return webPage.ToString();
        }

    }
}
