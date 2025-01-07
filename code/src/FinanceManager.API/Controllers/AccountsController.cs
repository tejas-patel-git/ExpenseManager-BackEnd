using FinanceManager.Application.Services;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers
{
    public class AccountsController : ApiController
    {
        private readonly IAccountsService _accountsService;
        private readonly IMapper<AccountsRequest, AccountsDomain> _requestDomainMapper;

        public AccountsController(IAccountsService accountsService,
                                  IMapper<AccountsRequest, AccountsDomain> requestDomainMapper)
        {
            _accountsService = accountsService;
            _requestDomainMapper = requestDomainMapper;
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] AccountsRequest accountsRequest)
        {
            var userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId)) return BadRequest(FailureResponse("User id is missing."));

            var accountsDomain = _requestDomainMapper.Map(accountsRequest);
            accountsDomain.UserId = userId;

            if (!await _accountsService.AddAccount(accountsDomain)) return Conflict(FailureResponse("User does not exists"));

            return Ok(SuccessResponse("Account added successfully."));
        }
    }
}
