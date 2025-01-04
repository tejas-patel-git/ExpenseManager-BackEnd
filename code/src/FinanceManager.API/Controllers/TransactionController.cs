using FinanceManager.Application.Services;
using FinanceManager.Data.Models;
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
        public async Task<IActionResult> GetTransactionById([FromQuery]Guid id)
        {
            // check if transaction id is provided
            if(id.Equals(Guid.Empty))
                return BadRequest(FailureResponse("Transaction id is empty."));

            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User Id is missing in the token.");
            }

            var transaction = await _transactionService.GetUserTransactionByIdAsync(id, userId);

            if (transaction == null)
                return NotFound(FailureResponse("No transaction found!"));

            return Ok(SuccessResponse(_domainResponseMapper.Map(transaction)));
        }

        /// <summary>
        /// Retrieves all transactions for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to filter transactions.</param>
        /// <returns>A collection of <see cref="Transaction"/> for the specified user.</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<TransactionResponse>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllTransactions(string userId)
        {
            var transactions = await _transactionService.GetAllTransactionsAsync(userId);

            if (!transactions.Any()) return NotFound(FailureResponse("No transaction found!"));

            return Ok(SuccessResponse(_domainResponseMapper.Map(transactions)));
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
            transaction.UserID = userId;

            if (!await _transactionService.AddTransactionAsync(transaction))
                return Conflict(FailureResponse("User does not exists"));

            return Ok(SuccessResponse(_domainResponseMapper.Map(transaction)));
        }

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to update.</param>
        /// <param name="transaction">The updated transaction entity.</param>
        /// <returns>A <see cref="IActionResult"/> representing the status of the operation.</returns>
        [HttpPut("{transactionId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTransaction(int transactionId, [FromBody] TransactionRequest transaction)
        {
            //if (transactionId != transaction.TransactionId)
            //    return BadRequest("Transaction ID mismatch.");

            // TODO : Add Validations

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _transactionService.UpdateTransactionAsync(transaction);
            return NoContent();
        }

        /// <summary>
        /// Deletes a transaction by its ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to delete.</param>
        /// <returns>A <see cref="IActionResult"/> indicating the deletion status.</returns>
        [HttpDelete("{transactionId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTransaction(Guid transactionId)
        {
            await _transactionService.DeleteTransactionAsync(transactionId);
            return NoContent();
        }


        private string? GetUserIdOfRequest()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
