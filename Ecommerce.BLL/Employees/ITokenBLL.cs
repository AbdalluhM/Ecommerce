using Microsoft.IdentityModel.Tokens;

using Ecommerce.Core.Entities;
using Ecommerce.DTO.Employees;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Employees
{
    public interface ITokenBLL
    {
        string BuildToken(Employee employee );

        string BuildToken( string key, string issuer, LoginModelOutputDto employee );
        bool IsTokenValid( string key, string issuer, string token );
    }
}
