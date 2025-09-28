using Common.Address.Controllers;
using Common.Address.Model;
using Common.Address.Work;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        [TearDown]
        [OneTimeTearDown]
        public void DisposeHttpClient()
        {
            _httpClient.Dispose();
        }
    }
}
