using Newtonsoft.Json;

namespace Ecommerce.Core.Helpers.JsonLanguages
{
    public class JsonLanguageModel
    {
        [JsonProperty("default")]
        public string Default { get; set; }

        [JsonProperty("ar")]
        public string Ar { get; set; }
    }

    public enum LangEnum
    {
        Default ,
        Ar
    }

  
}
