// -----------------------------------------------------------------------
// <copyright file="BinaryHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebServer.BusinessLogic.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.Sockets;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.IO; 

    /// <summary>
    /// handles binary requests.
    /// </summary>
    public class BinaryHandler : Handler
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
            if(request.HttpPath.EndsWith(".png") || request.HttpPath.EndsWith(".jpg") 
                || request.HttpPath.EndsWith(".jpeg") || request.HttpPath.EndsWith(".gif"))
            {
                string filePath = Configurator.Instance.RelativeWwwPath + request.HttpPath;
                
                if (!File.Exists(filePath))
                {
                    return this.NextHandler.Handle(request);
                }

                Response toReturn = new Response();
                toReturn.Data = ReadBytesFromFile(filePath);
                //tuReturn.Data = Encoding.ASCII.GetBytes(filePath);
                toReturn.MimeType = IdentifyContentType(request);
                return toReturn;
            }
            
                return this.NextHandler.Handle(request);
            }

        private byte[] ReadBytesFromFile(string filePath)
        {
            byte[] buffer;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int count;
                int sum = 0;
                long length = fs.Length;
                buffer = new byte[length];
                while ((count = fs.Read(buffer, sum, (int)(length - sum))) > 0)
                {
                    sum += count;
                }
            }

            return buffer;
        }
    }
    }

