using System.Text.Json.Serialization;

namespace Common.Address.Model.HerePlatform
{
    public class Item
    {
        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("resultType")]
        public required string ResultType { get; set; }

        [JsonPropertyName("localityType")]
        public string? LocalityType { get; set; }

        [JsonPropertyName("houseNumberType")]
        public required string HouseNumberType { get; set; }

        [JsonPropertyName("address")]
        public required Address Address { get; set; }

        [JsonPropertyName("position")]
        public required Position Position { get; set; }

        [JsonPropertyName("access")]
        public required List<Position> Access { get; set; }

        [JsonPropertyName("mapView")]
        public required MapView MapView { get; set; }

        [JsonPropertyName("scoring")]
        public required Scoring Scoring { get; set; }
    }
}
