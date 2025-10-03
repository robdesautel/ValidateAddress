using Common.Address.Enumerators;
using Common.Address.Model;
using Common.Address.Model.HerePlatform;
using Common.Address.Work;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Common.Address.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ValidateUserAddress : ControllerBase
    {

        private readonly ILogger<ValidateUserAddress> _logger;
        private HerePlatformValidator _herePlatformValidator;

        public ValidateUserAddress(ILogger<ValidateUserAddress> logger, HerePlatformValidator herePlatformValidator)
        {
            _logger = logger;
            _herePlatformValidator = herePlatformValidator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RootObject))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidateAddress(UserAddress userAddress)
        {
            var address = await _herePlatformValidator.ValidateAsync(userAddress);

            return Ok(address);
        }
        /// <summary>
        /// Assuming all postal code given will be US format
        /// 5 digit zip
        /// optional 9 digit zip with suffix of plus four
        /// </summary>
        /// <param name="postalCode"></param>
        /// <returns></returns>
        [HttpPost("{postalCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Error))]
        public async Task<IActionResult> ValidatePostalCode(string postalCode)
        {

            if (string.IsNullOrWhiteSpace(postalCode) ||
                postalCode.Length < 5 ||
                postalCode.Length > 10)
            {
                var error = new Error
                {
                    ErrorType = "Incorrect Format",
                    ErrorNumber = "1",
                    ErrorMessage = "Please enter a valid postal code to search."
                };
                return BadRequest(error);
            }
            var isPostalCode = await _herePlatformValidator.IsValidPostalCode(postalCode);

            if (isPostalCode is null) return BadRequest();

            return Ok();

        }

        [HttpPost("AddressSubquery/Dictionary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Error))]
        public async Task<IActionResult> AddressSubquery(Subquery subquery)
        {
            var result = await _herePlatformValidator.IsValidLocation(subquery);
            Error error;

            if (result is bool boolResult)
            {
                if (boolResult) return Ok();
                if (!boolResult)
                {
                    error = new Error
                    {
                        ErrorType = "Subqery returned no results.",
                        ErrorNumber = "1",
                        ErrorMessage = "Please enter a valid format."
                    };

                    return BadRequest(error);
                }
            }

            error = new Error
            {
                ErrorType = "Subqery returned no results.",
                ErrorNumber = "1",
                ErrorMessage = "Please enter a valid format."
            };

            return BadRequest(error);

        }

        [HttpPost("AddressSubquery/List")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Error))]
        public async Task<IActionResult> AddressSubquery(SubqueryList subqueries)
        {
            var result = await _herePlatformValidator.IsValidLocation(subqueries);
            Error error;

            if (result is bool boolResult)
            {
                if (boolResult) return Ok();
                if (!boolResult)
                {
                    error = new Error
                    {
                        ErrorType = "Subqery returned no results.",
                        ErrorNumber = "1",
                        ErrorMessage = "Please enter a valid format."
                    };

                    return BadRequest(error);
                }
            }

            error = new Error
            {
                ErrorType = "Subqery returned no results.",
                ErrorNumber = "1",
                ErrorMessage = "Please enter a valid format."
            };

            return BadRequest(error);

        }
    }
}
