using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace JF.NetTools.Restful
{
    public class RestClient
    {
        public RestClient(bool useCookies = false)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            if (useCookies)
            {
                Handler = new HttpClientHandler() { UseCookies = true };
                client = new HttpClient(Handler);
            }
            else
            {
                client = new HttpClient();
            }
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
        }
        public RestClient(string baseAddress, bool userCookies = false) : this(userCookies)
        {
            client.BaseAddress = new Uri(baseAddress);
        }

        private HttpClient client;

        public HttpClientHandler Handler { get; private set; }


        public string BaseAddress => client?.BaseAddress.AbsoluteUri;
        public System.Net.Http.Headers.HttpRequestHeaders DefaultRequestHeaders => client.DefaultRequestHeaders;

        /// <summary>
        /// Get a response by request specify url
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">Throw when request is failed</exception>
        public async Task<RestResponse<T>> Get<T>(string url) where T : class
        {
            var res = await client.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse<T>();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                r.Data = JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync());
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }

        public async Task<RestResponse<string>> Get(string url)
        {
            var res = await client.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse<string>();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                r.Data = await res.Content.ReadAsStringAsync();
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }

        public async Task<RestResponse<Stream>> GetStream(string url)
        {

            var res = await client.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse<Stream>();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                r.Data = await res.Content.ReadAsStreamAsync();
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }
        /// <summary>
        /// Post data in json format to specified url
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">Throw when request is failed</exception>
        public async Task<RestResponse<T>> Post<T>(string url, object data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType.MediaType = "application/json";
            content.Headers.ContentType.CharSet = "UTF-8";
            var res = await client.PostAsync(url, content);
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse<T>();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                r.Data = JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync());
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }
        public async Task<RestResponse<T>> Post<T>(string url, HttpContent content)
        {
            var res = await client.PostAsync(url, content);
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse<T>();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                r.Data = JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync());
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }
        public async Task<RestResponse> Post(string url, object data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType.MediaType = "application/json";
            content.Headers.ContentType.CharSet = "UTF-8";
            var res = await client.PostAsync(url, content);
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }
        public async Task<RestResponse> Post(string url, HttpContent content)
        {
            var res = await client.PostAsync(url, content);
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                return r;
            }
            throw new HttpRequestException(await res.Content.ReadAsStringAsync());
        }
        public async Task<RestResponse<T>> Put<T>(string url, object data)
        {
            var res = await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(data)));
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse<T>();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                r.Data = JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync());
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }
        public async Task<RestResponse> Put(string url, object data)
        {
            var res = await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(data)));
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }

        public async Task<RestResponse> Delete<T>(string url)
        {
            var res = await client.DeleteAsync(url);
            if (res.IsSuccessStatusCode)
            {
                var r = new RestResponse<T>();
                r.Headers = res.Headers;
                r.StatusCode = res.StatusCode;
                return r;
            }
            throw new RestRequestException(res.StatusCode, await res.Content.ReadAsStringAsync());
        }
    }
}