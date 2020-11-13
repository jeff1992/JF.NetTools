using System;
using System.Collections.Generic;
using System.Text;

namespace JF.NetTools.Wechat
{

    public class GetBaseTokenRes : WechatApiBaseRes
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
    public class GetWebAuthAccessTokenRes : WechatApiBaseRes
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
    }
    public class GetUserInfoRes : WechatApiBaseRes
    {
        public string openid { get; set; }
        public string nickname { get; set; }
        public int sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public string[] privilege { get; set; }
        public string unionid { get; set; }
    }
    public class GetComponentAccessTokenRes : WechatApiBaseRes
    {
        public string component_access_token { get; set; }
        public int expires_in { get; set; }
    }
    public class CreateComponentPreAuthCodeRes : WechatApiBaseRes
    {
        public string pre_auth_code { get; set; }
        public int expires_in { get; set; }
    }
}
