using Microsoft.VisualStudio.TestTools.UnitTesting;
using JF.NetTools.Restful;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JF.NetTools.Restful.Tests
{
    [TestClass()]
    public class RestClientTests
    {
        [TestMethod()]
        public async Task GetTest()
        {
            var client = new RestClient();
            var res = await client.Get("https://baidu.com");
            Assert.IsNotNull(res);
        }
    }
}