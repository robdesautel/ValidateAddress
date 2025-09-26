using System.Text.Json.Serialization;

namespace Common.Address.Model.HerePlatform
{
    public class FieldScore
    {
        [JsonPropertyName("country")]
        public required double Country { get; set; }

        [JsonPropertyName("state")]
        public required double State { get; set; }

        [JsonPropertyName("city")]
        public required double City { get; set; }

        [JsonPropertyName("streets")]
        public required List<double> Streets { get; set; }

        [JsonPropertyName("houseNumber")]
        public required double HouseNumber { get; set; }

        [JsonPropertyName("postalCode")]
        public required double PostalCode { get; set; }
    }
}
