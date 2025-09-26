using Common.Address.Model.HerePlatform;
using System.Text.Json.Serialization;

namespace Common.Address.Model
{
    public class Scoring
    {
        [JsonPropertyName("queryScore")]
        public required double QueryScore { get; set; }

        [JsonPropertyName("fieldScore")]
        public required FieldScore FieldScore { get; set; }
    }
}
