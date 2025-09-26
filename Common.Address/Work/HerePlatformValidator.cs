using Common.Address.Interface;
using Common.Address.Model;
using Newtonsoft.Json;

namespace Common.Address.Work
{
    public class HerePlatformValidator : IAddressValidator<RootObject>
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly string _geocodeEndpoint;

        public HerePlatformValidator(IConfiguration config, HttpClient httpClient)
        {
            _apiKey = config["Here:ApiKey"] ?? throw new Exception();
            _url = config["Here:BaseUri"] ?? throw new Exception();
            _geocodeEndpoint = config["Here:GeocodeEndpoint"] ?? throw new Exception();
            _httpClient = httpClient ?? throw new ArgumentNullException();
        }

        public async Task<RootObject?> ValidateAsync(UserAddress input)
        {
            var query = $"{input.Street} {input.City} {input.State} {input.ZipCode} {input.Country}";
            var requestUrl = $"{_url}{_geocodeEndpoint}?q={Uri.EscapeDataString(query)}&apiKey={_apiKey}&addressNamesMode=normalized";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RootObject>(json);


            return result;
        }

        private string GetComponent(dynamic addressComponents, string type)
        {
            foreach (var component in addressComponents)
            {
                foreach (var t in component.types)
                {
                    if (t == type)
                    {
                        return component.long_name;
                    }
                }
            }
            return string.Empty;
        }
    }
}
