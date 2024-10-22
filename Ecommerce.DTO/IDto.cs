using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO
{
    public interface IDto
    {
        //[JsonIgnore]
        //protected string Lang { get; set; }

    }
    public class AppleSignInDto
    {
        public string IdToken { get; set; }
        public string code { get; set; }
    }
}
