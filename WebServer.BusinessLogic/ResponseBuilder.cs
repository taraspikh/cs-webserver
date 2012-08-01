using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Is be able to provide raw data, needed for transfering it to the client
    /// </summary>
    public class ResponseBuilder
    {
        /// <summary>
        /// Response private containter with all necessary data for ResponseBuilder
        /// </summary>
        private Response _response;

        /// <summary>
        /// Constructor
        /// </summary>
        public ResponseBuilder(Response response)
        {
            _response = response;
        }

        /// <summary>
        /// Gets response data
        /// </summary>
        /// <returns>Response object that was initializeb by constructor</returns>
        public Response Response
        {
            get { return _response; }
        }

        /// <summary>
        /// Gets the raw bytes with all headers and body of response inside
        /// </summary>
        /// <returns>Bytes of raw data</returns>
        public byte[] GetResponseBytes()
        {
            //check if data is available
            if (_response.Data == null)
            {
                _response.HttpResponseCode = 500;
                _response.HttpResponseCodeDescription = "Handler error";
            }

            // create Header
            var headerCreator = new ResponseHeaderCreator(_response);
            headerCreator.Create();

            // get Header string value
            var header = headerCreator.ToString();

            // combine Header and Body
            byte[] result = CombineHeaderAndBody(header);

            // return combining result
            return result;
        }


        private byte[] CombineHeaderAndBody(string header)
        {
            // add CRLF to separate Header and Body
            header += "\r\n";

            // when we only have Header status, make Response.Data empty, not null so that combine is doesn't throw
            if (_response.Data == null)
            {
                _response.Data = new byte[0];
            }

            // get bytes of Header string
            var headerBytes = Encoding.UTF8.GetBytes(header);

            // combine bytes of Header with bytes of Body
            var newResult = Helpers.ByteArraysCombineExtension.Combine(headerBytes, _response.Data);

            return newResult;
        }
    }
}
