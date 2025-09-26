using System.Text.Json.Serialization;

namespace Common.Address.Model
{
    public class Position
    {
        [JsonPropertyName("lat")]
        public required double Lat { get; set; }

        [JsonPropertyName("lng")]
        public required double Lng { get; set; }
    }
}
