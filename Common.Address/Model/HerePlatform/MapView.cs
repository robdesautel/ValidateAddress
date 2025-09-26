using System.Text.Json.Serialization;

namespace Common.Address.Model.HerePlatform
{
    public class MapView
    {
        [JsonPropertyName("west")]
        public required double West { get; set; }

        [JsonPropertyName("south")]
        public required double South { get; set; }

        [JsonPropertyName("east")]
        public required double East { get; set; }

        [JsonPropertyName("north")]
        public required double North { get; set; }
    }
}
