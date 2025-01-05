using FinanceManager.Application.Services;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers
{
    /// <summary>
    /// Controller for managing transactions.
    /// </summary>
    public class TransactionController : ApiController
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;
        private readonly IMapper<TransactionRequest, TransactionDomain> _requestDomainMapper;
        private readonly IMapper<TransactionDomain, TransactionResponse> _domainResponseMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionController"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="transactionService">The transaction service to manage transactions.</param>
        /// <param name="requestDomainMapper"></param>
        /// <param name="domainResponseMapper"></param>
        public TransactionController(ILogger<TransactionController> logger,
                                     ITransactionService transactionService,
                                     IMapper<TransactionRequest, TransactionDomain> requestDomainMapper,
                                     IMapper<TransactionDomain, TransactionResponse> domainResponseMapper) : base()
        {
            _logger = logger;
            _transactionService = transactionService;
            _requestDomainMapper = requestDomainMapper;
            _domainResponseMapper = domainResponseMapper;
        }

        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="id">The ID of the transaction to retrieve.</param>
        /// <returns>
        /// A <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(Response<TransactionResponse>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        [ProducesResponseType(typeof(Response), 401)]
        [ProducesResponseType(typeof(Response), 404)]
        public async Task<IActionResult> GetTransactionById([FromQuery] Guid? id = null)
        {
            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User Id is missing in the token.");
            }

            if (id.HasValue)
            {
                // check if transaction id is provided
                if (id.Equals(Guid.Empty))
                    return BadRequest(FailureResponse("Invalid transaction id."));

                var transaction = await _transactionService.GetUserTransactionAsync(id.Value, userId);

                if (transaction == null)
                    return NotFound(FailureResponse("No transaction found!"));

                return Ok(SuccessResponse(_domainResponseMapper.Map(transaction)));
            }
            else
            {
                var transactions = await _transactionService.GetUserTransactionsAsync(userId);

                if (!transactions.Any()) return NotFound(FailureResponse("No transaction found!"));

                return Ok(SuccessResponse(_domainResponseMapper.Map(transactions)));
            }
        }

        /// <summary>
        /// Adds a new transaction.
        /// </summary>
        /// <param name="transactionRequest">The transaction entity to add.</param>
        /// <returns>A <see cref="IActionResult"/> with the status of the operation.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddTransaction([FromBody] TransactionRequest transactionRequest)
        {
            // TODO : Add Validations
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User Id is missing in the token.");
            }

            var transaction = _requestDomainMapper.Map(transactionRequest);
            transaction.UserId = userId;

            if (!await _transactionService.AddTransactionAsync(transaction))
                return Conflict(FailureResponse("User does not exists"));

            return Ok(SuccessResponse(_domainResponseMapper.Map(transaction)));
        }

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        /// <param name="id">The ID of the transaction to update.</param>
        /// <param name="transactionRequest">The updated transaction entity.</param>
        /// <returns>A <see cref="IActionResult"/> representing the status of the operation.</returns>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTransaction([FromQuery]Guid id, [FromBody] TransactionRequest transactionRequest)
        {
            // TODO : Add Validations

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User Id is missing in the token.");
            }

            var transactionDomain = _requestDomainMapper.Map(transactionRequest);
            transactionDomain.Id = id;
            transactionDomain.UserId = userId;

            await _transactionService.UpdateTransactionAsync(transactionDomain);
            return NoContent();
        }

        /// <summary>
        /// Deletes a transaction by its ID.
        /// </summary>
        /// <param name="id">The ID of the transaction to delete.</param>
        /// <returns>A <see cref="IActionResult"/> indicating the deletion status.</returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTransaction([FromQuery] Guid id)
        {
            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User id is missing in the token.");
            }

            var isSuccess = await _transactionService.DeleteTransactionAsync(id, userId);

            if (!isSuccess) return NotFound(FailureResponse("Transaction not found."));

            return Ok(SuccessResponse($"Transaction with id '{id}' deleted successfully."));
        }

        private string? GetUserIdOfRequest()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
