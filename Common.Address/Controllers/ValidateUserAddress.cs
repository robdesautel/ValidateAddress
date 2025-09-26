using Common.Address.Model;
using Common.Address.Work;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(RootObject))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidateAddress(UserAddress userAddress)
        {
            var address = await _herePlatformValidator.ValidateAsync(userAddress);

            return Ok(address);
        }
    }
}
