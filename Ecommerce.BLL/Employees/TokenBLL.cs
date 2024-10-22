using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Employees;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.DTO.Employees;
using Ecommerce.Helper.String;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Employees
{
   public class TokenBLL : /*BaseBLL*/ITokenBLL
    {
        private const double EXPIRY_DURATION_MINUTES = 60*24*7;
        private readonly IConfiguration _config;
        private readonly IRepository<Page> _pageRepository;
        private readonly IRepository<Role> _roleRepository;

        public TokenBLL(IConfiguration config, IRepository<Page> pageRepository, IRepository<Role> roleRepository)
        {
            _config = config;
            _pageRepository = pageRepository;
            _roleRepository = roleRepository;
        }


        public string BuildToken(Employee employee)
        {

            // generate token that is valid for 30 Minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config ["Jwt:Secret"].ToString());

            //add Employee Pages that have read access to
           var employeeViewPages = "";
            var role = _roleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x=>x.Employees.Contains(employee)).FirstOrDefault();
            if(role != null)
            {
                role.RolePageActions.ToList().ForEach(x =>
                {
                    if (x.PageAction.ActionId == (int)ActionsEnum.READ)
                        employeeViewPages += x.PageAction.PageId + ",";
                });
            }
           
            employeeViewPages = employeeViewPages.RemoveRedundant();
            //var pageIds = employeeViewPages.Split(',').Select(int.Parse).ToList();

            //var role = employee.Role;
            var pages = _pageRepository.GetAll();
            if (role == null && employee.IsAdmin == true && employee.IsAdmin != null)
            {
                foreach (var page in pages)
                {
                    employeeViewPages += page.Id + ",";
                }
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config ["Jwt:Issuer"].ToString(),
                Subject = new ClaimsIdentity(new []
                {
                    new Claim("Id", employee.Id.ToString()) ,
                    new Claim("UserName", employee.UserName),
                    new Claim("ReadPages", employeeViewPages),
                    new Claim(JwtRegisteredClaimNames.Sub, employee.UserName.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, employee.Email.ToString()),
                }

                ),
                Expires = DateTime.UtcNow.AddMinutes(EXPIRY_DURATION_MINUTES),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
        public string BuildToken( string key,string issuer, LoginModelOutputDto employee )
        {

            var claims = new [] {
                //new Claim("UserName", employee.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, employee.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim("EmployeeType", employee.EmployeeType),
                //new Claim("EmployeeTypeId", employee.EmployeeTypeId.ToString()),
                new Claim("Id", employee.Id.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }
        //public string GenerateJSONWebToken(string key, string issuer, UserDTO user)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(issuer, issuer,
        //      null,
        //      expires: DateTime.Now.AddMinutes(120),
        //      signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
        public bool IsTokenValid( string key, string issuer, string token )
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
