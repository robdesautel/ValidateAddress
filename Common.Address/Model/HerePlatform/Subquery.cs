using Common.Address.Enumerators;
using Common.Address.Helper;
using Newtonsoft.Json;
namespace Common.Address.Model.HerePlatform
{
    public class Subquery
    {

        [JsonProperty("subquery")]
        [JsonConverter(typeof(EnumKeyDictionaryConverter<SubqueryType, string>))]
        public required Dictionary<SubqueryType, string> SubqueryValue { get; set; }
    }
}
