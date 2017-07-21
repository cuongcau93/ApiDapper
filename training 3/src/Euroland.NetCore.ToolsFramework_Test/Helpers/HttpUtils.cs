using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Test.Helpers
{
    public static class HttpUtils
    {
        public static HttpContext MockHttpContext()
        {
            var httpContext = new Mock<HttpContext>();
            var httpRequest = new Mock<HttpRequest>();
            var queryString = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery("companyCode=companyTest");



            httpRequest.Setup(req => req.Query).Returns(new DummyHttpRequestQueryCollection(queryString));
            httpRequest.Setup(req => req.Form).Returns(new FormCollection(queryString));
            httpRequest.Setup(req => req.QueryString).Returns(new QueryString("?companyCode=companyTest"));

            httpRequest.Setup(req => req.HttpContext).Returns(httpContext.Object);

            httpContext.Setup(ctx => ctx.Request).Returns(httpRequest.Object);

            return httpContext.Object;
        }
    }
}
