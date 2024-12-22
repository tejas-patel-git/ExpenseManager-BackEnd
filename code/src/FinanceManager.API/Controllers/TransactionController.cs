using FinanceManager.Data.Models;
using FinanceManager.Models;
using FinanceManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers
{
    /// <summary>
    /// Controller for managing transactions.
    /// </summary>
    public class TransactionController : ApiController
    {
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionController"/> class.
        /// </summary>
        /// <param name="transactionService">The transaction service to manage transactions.</param>
        public TransactionController(ITransactionService transactionService) : base()
        {
            _transactionService = transactionService;
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
            if (transactionId <= 0)
                return BadRequest($"Transaction ID {transactionId} is invalid.");

            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        /// <summary>
        /// Retrieves all transactions for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to filter transactions.</param>
        /// <returns>A collection of <see cref="Transaction"/> for the specified user.</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), 200)]  
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllTransactions(int userId)
        {
            if (userId <= 0)
                return BadRequest($"User ID {userId} is invalid.");

            var transactions = await _transactionService.GetAllTransactionsAsync(userId);

            if (!transactions.Any()) return NotFound();

            return Ok(transactions);
        }

        /// <summary>
        /// Adds a new transaction.
        /// </summary>
        /// <param name="transaction">The transaction entity to add.</param>
        /// <returns>A <see cref="IActionResult"/> with the status of the operation.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Transaction), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddTransaction([FromBody] TransactionResponse transaction)
        {
            // TODO : Add Validatoins
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _transactionService.AddTransactionAsync(transaction);
            return CreatedAtAction(nameof(GetTransactionById), new { transactionId = transaction.TransactionID }, transaction);
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
        public async Task<IActionResult> UpdateTransaction(int transactionId, [FromBody] TransactionResponse transaction)
        {
            if (transactionId != transaction.TransactionID)
                return BadRequest("Transaction ID mismatch.");
            
            // TODO : Add Validatoins
            
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
