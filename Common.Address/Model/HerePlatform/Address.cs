using System.Text.Json.Serialization;

namespace Common.Address.Model.HerePlatform
{
    public class Address
    {
        [JsonPropertyName("label")]
        public required string Label { get; set; }

        [JsonPropertyName("countryCode")]
        public required string CountryCode { get; set; }

        [JsonPropertyName("countryName")]
        public required string CountryName { get; set; }

        [JsonPropertyName("stateCode")]
        public required string StateCode { get; set; }

        [JsonPropertyName("state")]
        public required string State { get; set; }

        [JsonPropertyName("county")]
        public required string County { get; set; }

        [JsonPropertyName("city")]
        public required string City { get; set; }

        [JsonPropertyName("street")]
        public required string Street { get; set; }

        [JsonPropertyName("postalCode")]
        public required string PostalCode { get; set; }

        [JsonPropertyName("houseNumber")]
        public required string HouseNumber { get; set; }
    }
}
