using FinanceManager.Application.Services;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers
{
    // <summary>
    /// Controller for managing savings goals.
    /// </summary>
    public class SavingsController : ApiController
    {
        private readonly ILogger<SavingsController> _logger;
        private readonly ISavingsService _savingsService;
        private readonly IMapper<SavingsRequest, SavingsGoalDomain> _requestDomainMapper;
        private readonly IMapper<SavingsGoalDomain, SavingsResponse> _domainResponseMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SavingsController"/> class.
        /// </summary>
        public SavingsController(
            ILogger<SavingsController> logger,
            ISavingsService savingsService,
            IMapper<SavingsRequest, SavingsGoalDomain> requestDomainMapper,
            IMapper<SavingsGoalDomain, SavingsResponse> domainResponseMapper) : base()
        {
            _logger = logger;
            _savingsService = savingsService;
            _requestDomainMapper = requestDomainMapper;
            _domainResponseMapper = domainResponseMapper;
        }

        /// <summary>
        /// Retrieves a savings goal by its ID or all savings goals for the user if no ID is provided.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Response<SavingsResponse>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        [ProducesResponseType(typeof(Response), 401)]
        [ProducesResponseType(typeof(Response), 404)]
        public async Task<IActionResult> GetSavings([FromQuery] Guid? id = null)
        {
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(FailureResponse("User Id is missing in the token."));
            }

            if (id.HasValue)
            {
                if (id.Equals(Guid.Empty))
                    return BadRequest(FailureResponse("Invalid savings id."));

                var savings = await _savingsService.GetUserSavingsAsync(id.Value, userId);
                if (savings == null)
                    return NotFound(FailureResponse("No savings found!"));

                return Ok(SuccessResponse(_domainResponseMapper.Map(savings)));
            }
            else
            {
                var savings = await _savingsService.GetUserSavingsAsync(userId);
                if (savings.Count == 0)
                    return NotFound(FailureResponse("No savings goals found!"));

                return Ok(SuccessResponse(_domainResponseMapper.Map(savings)));
            }
        }

        /// <summary>
        /// Creates a new savings goal.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SavingsResponse), 201)]
        [ProducesResponseType(typeof(Response), 400)]
        [ProducesResponseType(typeof(Response), 401)]
        public async Task<IActionResult> CreateSavings([FromBody] SavingsRequest savingsRequest)
        {
            _logger.LogInformation("Request received for creating a new savings goal.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(FailureResponse("User Id is missing in the token."));
            }
            else if (!await _savingsService.UserExists(userId))
            {
                return BadRequest(FailureResponse("User does not exist"));
            }

            var savingsDomain = _requestDomainMapper.Map(savingsRequest);
            savingsDomain.UserId = userId;

            var created = await _savingsService.AddSavingsAsync(savingsDomain);

            _logger.LogInformation("Request completed for creating a new savings goal.");
            return CreatedAtAction(nameof(GetSavings),
                new { id = created.Id },
                SuccessResponse(_domainResponseMapper.Map(created)));
        }

        /// <summary>
        /// Updates an existing savings goal.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Response), 400)]
        [ProducesResponseType(typeof(Response), 401)]
        [ProducesResponseType(typeof(Response), 404)]
        public async Task<IActionResult> UpdateSavings(
            [FromQuery] Guid id,
            [FromBody] SavingsRequest savingsRequest)
        {
            _logger.LogInformation("Received savings goal update request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(FailureResponse("User Id is missing in the token."));
            }
            else if (!await _savingsService.UserExists(userId))
            {
                return BadRequest(FailureResponse("User does not exist"));
            }

            if (!await _savingsService.Exists(id))
            {
                return NotFound(FailureResponse("Savings goal does not exist"));
            }

            var savingsDomain = _requestDomainMapper.Map(savingsRequest);
            savingsDomain.Id = id;
            savingsDomain.UserId = userId;

            await _savingsService.UpdateSavingsAsync(savingsDomain);

            _logger.LogInformation("Savings goal update request fulfilled");
            return NoContent();
        }

        /// <summary>
        /// Deletes a savings goal by its ID.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Response), 400)]
        [ProducesResponseType(typeof(Response), 404)]
        public async Task<IActionResult> DeleteSavings([FromQuery] Guid id)
        {
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(FailureResponse("User Id is missing in the token."));
            }

            var isSuccess = await _savingsService.DeleteSavingsAsync(id, userId);
            if (!isSuccess)
                return NotFound(FailureResponse("Savings goal not found."));

            return Ok(SuccessResponse($"Savings goal with id '{id}' deleted successfully."));
        }
    }
}
