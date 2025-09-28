using System.Text.Json.Serialization;
using Newtonsoft.Json;
namespace Common.Address.Model.HerePlatform
{
    public class SubqueryList
    {
        [JsonProperty("items")]
        public required List<Subquery> Subquery { get; set; }
    }
}
