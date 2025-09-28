using Common.Address.Enumerators;
using Common.Address.Interface;
using Common.Address.Model;
using Common.Address.Model.HerePlatform;
using Newtonsoft.Json;

namespace Common.Address.Work
{
    public class HerePlatformValidator : IAddressValidator<RootObject>
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly string _url = string.Empty;
        private readonly string _geocodeEndpoint;
        private string BaseQuery => $"{_url}{_geocodeEndpoint}?apiKey={_apiKey}&lang=eng";

        public HerePlatformValidator(IConfiguration config, HttpClient httpClient)
        {
            _apiKey = Environment.GetEnvironmentVariable("ADDRESS_API_KEY", EnvironmentVariableTarget.User) ?? throw new Exception("ADDRESS_API_KEY was not set in the environment please run exe file to set this up.");
            _url = config["Here:BaseUri"] ?? throw new Exception();
            _geocodeEndpoint = config["Here:GeocodeEndpoint"] ?? throw new Exception();
            _httpClient = httpClient ?? throw new ArgumentNullException();
        }

        public async Task<RootObject?> ValidateAsync(UserAddress input)
        {
            var query = $"{input.Street} {input.City} {input.State} {input.ZipCode} {input.Country}";
            var requestUrl = $"{BaseQuery}&q ={Uri.EscapeDataString(query)}&addressNamesMode=normalized";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RootObject>(json);


            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postalCode"></param>
        /// <returns></returns>
        public async Task<bool?> IsValidPostalCode(string postalCode) {

            var requestUrl = $"{BaseQuery}?q={Uri.EscapeDataString(postalCode)}";
            
            var response = await _httpClient.GetAsync(requestUrl);            
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RootObject>(json);

            return result?.Items
                    .Where(p => p.LocalityType.Equals("postalcode", StringComparison.InvariantCultureIgnoreCase) &&
                        p.Address.CountryCode.Equals("USA", StringComparison.InvariantCultureIgnoreCase) &&
                        p.Address.PostalCode.Equals(postalCode)).FirstOrDefault() is not null;
        }

        public async Task<bool?> IsValidLocation(List<Dictionary<SubqueryType, string>> subQueries)
        {
            string q = "qq=country=United State;";
            string? subq = string.Empty;

            foreach (var subqueries in subQueries)
            {
                subq += string.Join(";",ConcatSubQuery(subqueries));
            }

            q += subq;
            var result = await QueryResult<RootObject>(q) as RootObject;

            return result?.Items.Count > 0;
        }

        public async Task<bool?> IsValidLocation(SubqueryList subQueries)
        {
            string q = "qq=country=United States";
            string subq = string.Empty;

            foreach (var subQuery in subQueries.Subquery)
            {
                subq += string.Join(";",ConcatSubQuery(subQuery.SubqueryValues));
            }

            q += subq;
            var result = await QueryResult<RootObject>(q) as RootObject;
            return result?.Items.Count > 0;
        }

        private string ConcatSubQuery(Dictionary<SubqueryType, string> subQuery)
        {
            string subq = string.Empty;
            foreach (var subquery in subQuery)
            {
                subq += $"{subquery.Key}={subquery.Value}";
            }
            return subq;
        }

        private async Task<object?> QueryResult<T>(string query)
        {
            var q = $"{BaseQuery}&{query}";
            var request = await _httpClient.GetAsync(q);

            if (!request.IsSuccessStatusCode) return null;

            var content = await request.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<T>(content);

            return response;
        }

    }
}
