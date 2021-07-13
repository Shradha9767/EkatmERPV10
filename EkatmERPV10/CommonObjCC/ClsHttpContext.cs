using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace EkatmERPV10.CommonObjCC
{
    public class ClsHttpContext
    {
        public string showURL(IHttpContextAccessor httpcontextaccessor)
        {
            var request = httpcontextaccessor.HttpContext.Request;
            var absoluteUri = string.Concat(
                        request.Scheme,
                        "://",
                        request.Host.ToUriComponent(),
                        request.PathBase.ToUriComponent(),
                        request.Path.ToUriComponent(),
                        request.QueryString.ToUriComponent());
            return absoluteUri;
        }
    }
}
