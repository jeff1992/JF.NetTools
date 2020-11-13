using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace JF.NetTools.Restful
{
    public class RestRequestException : Exception
    {
        string message;
        public RestRequestException(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            this.message = message;
        }
        public HttpStatusCode StatusCode { get; private set; }
        public override string Message => message;
    }
}
