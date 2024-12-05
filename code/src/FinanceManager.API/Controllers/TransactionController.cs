using FinanceManager.Models;
using FinanceMangement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FinanceManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions()
        {
            var transactions = await _service.GetAllTransactionsAsync();

            if (transactions.IsNullOrEmpty()) return NoContent();

            return Ok(transactions);
        }

        [HttpGet("[action]/{userID}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions(int userID)
        {
            var transactions = await _service.GetAllTransactionsAsync(userID);

            if (transactions.IsNullOrEmpty()) return NoContent();

            return Ok(transactions);
        }
    }
}
