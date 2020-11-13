using System;
using System.Collections.Generic;
using System.Text;

namespace JF.NetTools.Wechat
{

    public class WechatApiException : Exception
    {
        public WechatApiException(WechatApiBaseRes data)
        {
            ResponseData = data;
        }
        public WechatApiBaseRes ResponseData { get; private set; }

        public override string Message => $"wechat api failed {nameof(ResponseData.errcode)}: {ResponseData.errcode}, {nameof(ResponseData.errmsg)}: {ResponseData.errmsg}";
    }
}
