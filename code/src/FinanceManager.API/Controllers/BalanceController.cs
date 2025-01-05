using FinanceManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers
{
    public class BalanceController : ApiController
    {
        private readonly ITransactionService _transactionService;

        public BalanceController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentBalance()
        {
            var user = GetUserIdOfRequest();
            if(string.IsNullOrEmpty(user)) return BadRequest(FailureResponse("User id is missing."));

            var balance = await _transactionService.GetBalanceAsync(user);
            if(balance == null) return NotFound(FailureResponse("No transactions found."));

            return Ok(SuccessResponse(balance));
        }
    }
}
