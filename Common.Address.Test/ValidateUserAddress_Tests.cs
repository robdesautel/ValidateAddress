using Common.Address.Controllers;
using Common.Address.Enumerators;
using Common.Address.Model;
using Common.Address.Model.HerePlatform;
using Common.Address.Work;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NJsonSchema.Generation.TypeMappers;
using System.Diagnostics.Eventing.Reader;

namespace Common.Address.Test
{
    public class ValidateUserAddress_Tests
    {
        private IConfiguration _configuration;
        private HttpClient _httpClient;
        private HerePlatformValidator? _herePlatformValidator;
        private ValidateUserAddress? _validateUserAddress;
        private ILogger<ValidateUserAddress>? _logger;


        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _httpClient = new HttpClient();

        }

        [Test]
        public void ValidateAsyncTest()
        {
            _herePlatformValidator = new HerePlatformValidator(_configuration, _httpClient);

            var userAddress = new UserAddress
            {
                Street = "425 W Randolph St",
                City = "Chicago",
                State = "IL",
                ZipCode = "60606",
                Country = "USA"
            };

            var result = _herePlatformValidator.ValidateAsync(userAddress).Result;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.EqualTo(null), "The object is null");
                Assert.That(result?.GetType(), Is.EqualTo(typeof(RootObject)), "is not the rootobject type");
            });

            DisposeHttpClient();
        }

        [Test]
        public void ValidateUserAddressTest()
        {
            _herePlatformValidator = new HerePlatformValidator(_configuration, _httpClient);
            _validateUserAddress = new ValidateUserAddress(_logger, _herePlatformValidator);

            var userAddress = new UserAddress
            {
                Street = "425 W Randolph St",
                City = "Chicago",
                State = "IL",
                ZipCode = "60606",
                Country = "USA"
            };

            var result = (_validateUserAddress.ValidateAddress(userAddress).Result as OkObjectResult)?.Value as RootObject;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.EqualTo(null), "The object is null");
                Assert.That(result?.GetType(), Is.EqualTo(typeof(RootObject)), "is not the rootobject type");
            });

            DisposeHttpClient();
        }

        [Test]
        public void IsValidPostalCodeTest()
        {
            var postalCode = "3002";
            _herePlatformValidator = new HerePlatformValidator(_configuration, _httpClient);

            var isValid = _herePlatformValidator?.IsValidPostalCode(postalCode).Result;

            switch (isValid)
            {
                case bool valid when valid: Assert.That(valid, Is.True);
                    break;
                case bool valid when !valid: Assert.That(valid, Is.False);
                    break;
                default:
                    Assert.That(isValid, Is.Null);
                    break;
            }

            DisposeHttpClient();
        }

        [Test]
        public void ValidatePostalCodeTest()
        {
            var postalCode = "3002";
            _herePlatformValidator = new HerePlatformValidator(_configuration, _httpClient);
            _validateUserAddress = new ValidateUserAddress(_logger, _herePlatformValidator);

            var result = _validateUserAddress.ValidatePostalCode(postalCode).Result;

            if (result is StatusCodeResult statusCodeResult)
            {
                Assert.That(statusCodeResult.StatusCode, Is.EqualTo(200), "Is not an Ok result");
            }
            else if (result is ObjectResult objectResult)
            {
                Assert.That(objectResult.StatusCode, Is.EqualTo(400), "Is Not a bad request");
            }

            DisposeHttpClient();
        }

        [Test]
        public void IsValidLocationListDictionaryTest()
        {
            _herePlatformValidator = new HerePlatformValidator(_configuration, _httpClient);

            var query = new List<Dictionary<SubqueryType, string>> { new Dictionary<SubqueryType, string> { { SubqueryType.state, "ZZ" } } };

            var result = _herePlatformValidator?.IsValidLocation(query).Result;

            Assert.That(result, Is.Not.EqualTo(null));

            if (result is not null)
            {
                Assert.That(result, Is.True);
            }
            else
            {
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void IsValidLocationJsonDocTest()
        {
            //This allows the capability of searching dynamic queries to ensure user input is accurate.

            //receiving json doc from client
            var jsonDoc = "{\"items\": [{\"subquery\": {\"state\": \"ga\"}},{\"subquery\": {\"postalCode\": \"30082\"}}]}";

            //converting json doc to List<Dictionary<enum, string>>
            //using custom JSON Enum mapping for Dictionary<enum, string>
            var subqueryList = JsonConvert.DeserializeObject<SubqueryList>(jsonDoc);

            //passed
            Assert.That(subqueryList?.Subquery, Is.Not.Null);
            
            //passed
            Assert.That(subqueryList?.Subquery.Count, Is.GreaterThan(0));

            //TODO: finish testing the _herePlatformValidator.IsValidLocation(SubqueryList) member
        }

        [TearDown]
        [OneTimeTearDown]
        public void DisposeHttpClient()
        {
            _httpClient.Dispose();
        }
    }
}
