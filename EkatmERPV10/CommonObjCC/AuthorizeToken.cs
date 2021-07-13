using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EkatmERPV10.CommonObjCC
{
    public class AuthorizeToken
    {
        IConfiguration _config;

        public AuthorizeToken(IConfiguration config)
        {
            _config = config;
        }
       
    }
}
