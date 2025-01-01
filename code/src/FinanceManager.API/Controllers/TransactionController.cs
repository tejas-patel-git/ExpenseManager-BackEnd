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
        private readonly ITransactionService _transactionService;
        private readonly IMapper<TransactionRequest, TransactionDomain> _requestDomainMapper;
        private readonly IMapper<TransactionDomain, TransactionResponse> _domainResponseMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionController"/> class.
        /// </summary>
        /// <param name="transactionService">The transaction service to manage transactions.</param>
        /// <param name="requestDomainMapper"></param>
        public TransactionController(ITransactionService transactionService, IMapper<TransactionRequest, TransactionDomain> requestDomainMapper, IMapper<TransactionDomain, TransactionResponse> domainResponseMapper) : base()
        {
            _transactionService = transactionService;
            _requestDomainMapper = requestDomainMapper;
            _domainResponseMapper = domainResponseMapper;
        }

        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to retrieve.</param>
        /// <returns>
        /// A <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpGet("{transactionId}")]
        [ProducesResponseType(typeof(TransactionResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTransactionById(int transactionId)
        {

            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);

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
        public async Task<IActionResult> GetAllTransactions(int userId)
        {
            if (userId <= 0)
                return BadRequest($"User ID {userId} is invalid.");

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

            var transaction = _requestDomainMapper.Map(transactionRequest);
            await _transactionService.AddTransactionAsync(transaction);
            return CreatedAtAction(nameof(GetTransactionById), new { transactionId = transaction.Id }, transaction);
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
            if (transactionId != transaction.TransactionId)
                return BadRequest("Transaction ID mismatch.");

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
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            await _transactionService.DeleteTransactionAsync(transactionId);
            return NoContent();
        }
    }
}
