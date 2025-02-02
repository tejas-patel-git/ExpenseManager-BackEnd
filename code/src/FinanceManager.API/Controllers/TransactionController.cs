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
        private readonly IAccountsService _accountService;
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
                                     IAccountsService accountService,
                                     IMapper<TransactionRequest, TransactionDomain> requestDomainMapper,
                                     IMapper<TransactionDomain, TransactionResponse> domainResponseMapper) : base()
        {
            _logger = logger;
            _transactionService = transactionService;
            _accountService = accountService;
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
            _logger.LogInformation("Request received for adding a new transaction.");

            // TODO : Add Validations
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(FailureResponse("User id is missing in the token."));
            }
            else if (!await _transactionService.UserExists(userId))
            {
                return BadRequest(FailureResponse("User does not exists"));
            }

            // check if payment exists
            if (transactionRequest.Payments == null) return BadRequest(FailureResponse("Payment information is missing."));

            // check if request have payment accounts
            var accountIds = transactionRequest.Payments.Accounts?.Where(acc => acc.AccountId != Guid.Empty)
                                                                   .Select(acc => acc.AccountId)
                                                                   .ToList();
            if (accountIds == null || accountIds.Count == 0)
            {
                return BadRequest(FailureResponse("Payment account is missing."));
            }

            // validate if payment account amount match with the transaction amount
            var amount = transactionRequest.Payments.Accounts.Sum(x => x.Amount);
            if(transactionRequest.Amount != amount) return BadRequest(FailureResponse("Transaction & total payment account amount mismatch."));


            // validate if payment account exists
            if (!await _accountService.Exists(accountIds, userId))
            {
                if (accountIds.Count > 1)
                    return BadRequest(FailureResponse("Not all payment account exists."));
                else
                    return BadRequest(FailureResponse("Payment account does not exist."));
            }

            // request model to domain model mapping
            var transactionDomain = _requestDomainMapper.Map(transactionRequest);
            transactionDomain.UserId = userId;

            // add transaction
            var transaction = await _transactionService.AddTransactionAsync(transactionDomain);

            _logger.LogInformation("Request completed for adding a new transaction.");
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
        public async Task<IActionResult> UpdateTransaction([FromQuery] Guid id, [FromBody] TransactionRequest transactionRequest)
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
            else if (!await _transactionService.UserExists(userId))
            {
                return BadRequest(FailureResponse("User does not exists"));
            }

            // check if payment exists
            if (transactionRequest.Payments == null) return BadRequest(FailureResponse("Payment information is missing."));

            // check if request have payment accounts
            var accountIds = transactionRequest.Payments.Accounts?.Where(acc => acc.AccountId != Guid.Empty)
                                                                  .Select(acc => acc.AccountId)
                                                                  .ToList();
            if (accountIds == null || accountIds.Count == 0)
            {
                return BadRequest(FailureResponse("Payment account is missing."));
            }

            // validate if payment account amount match with the transaction amount
            var amount = transactionRequest.Payments?.Accounts?.Sum(x => x.Amount);
            if (transactionRequest.Amount != amount) return BadRequest(FailureResponse("Transaction & total payment account amount mismatch."));

            // validate if payment account exists
            if (!await _accountService.Exists(accountIds, userId))
            {
                if (accountIds.Count > 1)
                    return BadRequest(FailureResponse("Not all payment account exists."));
                else
                    return BadRequest(FailureResponse("Payment account does not exist."));
            }

            // map transaction to domain
            var transactionDomain = _requestDomainMapper.Map(transactionRequest);
            transactionDomain.Id = id;
            transactionDomain.UserId = userId;
            foreach (var payment in transactionDomain.Payments) payment.TransactionId = transactionDomain.Id;

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
    }
}
