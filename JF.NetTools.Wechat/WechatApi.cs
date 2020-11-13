using JF.NetTools.Restful;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JF.NetTools.Wechat
{
    public class WechatApi
    {
        public WechatApi()
        {
            client = new RestClient();
        }
        RestClient client;

        public async Task<T> Get<T>(string url) where T : WechatApiBaseRes
        {
            var res = await client.Get<T>(url);
            if (res.Data.errcode == 0)
                return res.Data;
            throw new WechatApiException(res.Data);
        }

        /// <summary>
        /// Request wechat url by post and get specify type response data
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="WechatApiException"></exception>
        public async Task<T> Post<T>(string url, object data) where T : WechatApiBaseRes
        {
            var res = await client.Post<T>(url, data);
            if (res.Data.errcode == 0)
                return res.Data;
            throw new WechatApiException(res.Data);
        }

        /// <summary>
        /// Post data in json format to wechat
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="WechatApiException">Throw when wechat return fault code</exception>
        public async Task<WechatApiBaseRes> Post(string url, object data)
        {
            var res = await client.Post<WechatApiBaseRes>(url, data);
            if (res.Data.errcode == 0)
                return res.Data;
            throw new WechatApiException(res.Data);
        }

        /// <summary>
        /// Post specified http content to wechat
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="WechatApiException">Throw when wechat return fault code</exception>
        public async Task<WechatApiBaseRes> Post(string url, HttpContent content)
        {
            var res = await client.Post<WechatApiBaseRes>(url, content);
            if (res.Data.errcode == 0)
                return res.Data;
            throw new WechatApiException(res.Data);
        }

        /// <summary>
        /// 获取网页授权AccessToken
        /// </summary>
        /// <param name="code"></param>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public async Task<GetWebAuthAccessTokenRes> GetWebAuthAccessToken(string code, string appId, string appSecret)
        {
            return await Get<GetWebAuthAccessTokenRes>($"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appId}&secret={appSecret}&code={code}&grant_type=authorization_code");
        }
        /// <summary>
        /// 获取网页授权的用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<GetUserInfoRes> GetUserInfo(string accessToken, string openid)
        {
            return await Get<GetUserInfoRes>($"https://api.weixin.qq.com/sns/userinfo?access_token={accessToken}&openid={openid}&lang=zh_CN");
        }
        /// <summary>
        /// 获取access_token，会使上一次获取的token在一段时间之后失效
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public async Task<GetBaseTokenRes> GetBaseToken(string appId, string appSecret)
        {
            return await Get<GetBaseTokenRes>($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={appSecret}");
        }
        /// <summary>
        /// 获取第三方平台component_access_token
        /// </summary>
        /// <param name="appId">第三方平台appid</param>
        /// <param name="appSecret">第三方平台appsecret</param>
        public async Task<GetComponentAccessTokenRes> GetComponentAccessToken(string appId, string appSecret, string verifyTicket)
        {
            return await Get<GetComponentAccessTokenRes>($"https://api.weixin.qq.com/cgi-bin/component/api_component_token?component_appid={appId}&component_appsecret={appSecret}&component_verify_ticket=verifyTicket");
        }
        /// <summary>
        /// 创建第三方平台预授权码
        /// </summary>
        /// <param name="componentAppId">第三方平台appid</param>
        /// <param name="componentAccessToken">第三方平台component_access_token</param>
        public async Task<CreateComponentPreAuthCodeRes> CreateComponentPreAuthCode(string componentAppId, string componentAccessToken)
        {
            return await Post<CreateComponentPreAuthCodeRes>($"https://api.weixin.qq.com/cgi-bin/component/api_create_preauthcode?component_access_token={componentAccessToken}", new
            {
                component_appid = componentAppId
            });
        }

        /// <summary>
        /// 调用微信服务验证媒体内容是否合法
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileName"></param>
        /// <param name="fileStraem"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        /// <exception cref="WechatApiException"></exception>
        public async Task<bool> ValidateMedia(string accessToken, string fileName, Stream fileStraem, string contentType)
        {
            if (fileStraem == null)
                throw new ArgumentNullException(nameof(fileStraem));

            var content = new MultipartFormDataContent();
            var stContent = new StreamContent(fileStraem);
            stContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data");
            stContent.Headers.ContentDisposition.Name = "\"media\"";
            stContent.Headers.ContentDisposition.FileName = "\"" + fileName + "\"";
            stContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            content.Add(stContent);

            //httpclient 的 boundary 的值会自动携带双引号，而postman发送没有，微信服务器是按照无双引号处理的，故需要将双引号去掉
            var boundaryValue = content.Headers.ContentType.Parameters.Single(p => p.Name == "boundary");
            boundaryValue.Value = boundaryValue.Value.Replace("\"", String.Empty);

            try
            {
                await Post($"https://api.weixin.qq.com/wxa/img_sec_check?access_token={accessToken}", content);
                return true;
            }
            catch (WechatApiException e)
            {
                if (e.ResponseData.errcode == 87014)
                    return false;
                throw e;
            }
        }

        /// <summary>
        /// 调用微信服务器验证文本是否合法
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true when value parameter is lawful</returns>
        /// <exception cref="WechatApiException">Throw when wechat api return fault code</exception>
        public async Task<bool> ValidateText(string accessToken, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return true;
            try
            {
                await Post($"https://api.weixin.qq.com/wxa/msg_sec_check?access_token={accessToken}", new { content = text });
                return true;
            }
            catch (WechatApiException e)
            {
                if (e.ResponseData.errcode == 87014)
                    return false;
                throw e;
            }
        }
    }
}
