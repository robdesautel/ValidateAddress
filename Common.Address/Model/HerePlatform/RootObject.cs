using Common.Address.Model.HerePlatform;
using System.Text.Json.Serialization;

namespace Common.Address.Model
{
    public class RootObject
    {
        [JsonPropertyName("items")]
        public required List<Item> Items { get; set; }
    }
}
