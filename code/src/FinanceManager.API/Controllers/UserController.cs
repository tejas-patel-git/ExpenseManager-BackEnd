using FinanceManager.Application.Services;
using FinanceManager.Configuration;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers
{
    // TODO : Revisit

    /// <summary>
    /// Controller for managing user-related operations.
    /// </summary>
    public class UserController : ApiController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper<UserRegistrationRequest, UserDomain> _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The service for user-related business logic.</param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public UserController(IUserService userService, ILogger<UserController> logger, IMapper<UserRegistrationRequest, UserDomain> mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [Authorize(ConfigurationConstants.API_KEY_AUTH_SCHEME)]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest request)
        {
            _logger.LogInformation("Received new user registration: {UserId}", request.UserId);

            if (request is null || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(FailureResponse("Invalid user data"));
            }

            var isSuccess = await _userService.CreateUserAsync(_mapper.Map(request));

            if (!isSuccess)
            {
                _logger.LogInformation("User with email {email} already exists.", request.Email);
                return Conflict(FailureResponse("User already exists."));
            }

            return Ok(SuccessResponse("User registered successfully"));
        }
    }

}
