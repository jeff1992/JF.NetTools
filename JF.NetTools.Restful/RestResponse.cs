using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace JF.NetTools.Restful
{
    public class RestResponse
    {
        public HttpStatusCode StatusCode { get; internal set; }
        public HttpResponseHeaders Headers { get; internal set; }
    }
    public class RestResponse<T> : RestResponse
    {
        public T Data { get; internal set; }
    }

}
