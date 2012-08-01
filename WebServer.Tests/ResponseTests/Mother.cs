using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.BusinessLogic;

namespace WebServer.Tests.ResponseTests
{
    public class Mother
    {
        public static Response CreateSampleResponse()
        {
            //create sample data for Response
            var data = Encoding.UTF8.GetBytes("This is just a test data!");

            //create Response
            var r = new Response("text/html", WebServer.BusinessLogic.Helpers.WebserverConstants.HttpVersion10, data);

            return r;
        }
    }
}
