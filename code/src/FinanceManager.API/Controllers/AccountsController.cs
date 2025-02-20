using FinanceManager.Application.Services;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers
{
    public class AccountsController : ApiController
    {
        private readonly IAccountsService _accountsService;
        private readonly IMapper<AccountsRequest, AccountsDomain> _requestDomainMapper;
        private readonly IMapper<AccountsDomain, AccountsResponse> _domainResponseMapper;

        public AccountsController(IAccountsService accountsService,
                                  IMapper<AccountsRequest, AccountsDomain> requestDomainMapper,
                                  IMapper<AccountsDomain, AccountsResponse> domainResponseMapper)
        {
            _accountsService = accountsService;
            _requestDomainMapper = requestDomainMapper;
            _domainResponseMapper = domainResponseMapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount([FromQuery] Guid? id)
        {
            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(FailureResponse("User Id is missing in the token."));
            }

            // check if an account id is provided
            if (id.HasValue)
            {
                // return an account if exists

                if (id.Equals(Guid.Empty))
                    return BadRequest(FailureResponse("Invalid account id."));

                var accountsDomain = await _accountsService.GetAccounts(id.Value, userId);

                if (accountsDomain == null)
                    return NotFound(FailureResponse("Account does not exist."));

                return Ok(SuccessResponse(_domainResponseMapper.Map(accountsDomain)));
            }
            else
            {
                // return list of accounts of user if exists

                var accountsDomain = await _accountsService.GetAccounts(userId);

                if (!accountsDomain.Any())
                    return NotFound(FailureResponse("Account does not exist."));

                return Ok(SuccessResponse(_domainResponseMapper.Map(accountsDomain)));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] AccountsRequest accountsRequest)
        {
            var userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId)) return BadRequest(FailureResponse("User id is missing."));

            if (await AccountNameExists(userId, accountsRequest.AccountName)) return Conflict(FailureResponse("Account name already exists"));

            var accountsDomain = _requestDomainMapper.Map(accountsRequest);
            accountsDomain.UserId = userId;

            if (!await _accountsService.AddAccount(accountsDomain)) return Conflict(FailureResponse("User does not exists"));

            return Ok(SuccessResponse(_domainResponseMapper.Map(accountsDomain)));
        }

        private async Task<bool> AccountNameExists(string userId, string accountName)
        {
            if (await _accountsService.Exists(userId, accountName)) return true;

            return false;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromQuery] Guid id, [FromBody] AccountsRequest accountRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // validate id
            if (id.Equals(Guid.Empty))
                return BadRequest(FailureResponse("Invalid account id."));

            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User Id is missing in the token.");

            var accountsDomain = _requestDomainMapper.Map(accountRequest);
            accountsDomain.Id = id;
            accountsDomain.UserId = userId;

            var isSuccess = await _accountsService.UpdateAccountAsync(accountsDomain);
            if (!isSuccess) return NotFound(FailureResponse("Account does not exist."));

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransaction([FromQuery] Guid id)
        {
            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User id is missing in the token.");
            }

            var isSuccess = await _accountsService.DeleteTransactionAsync(id, userId);

            if (!isSuccess) return NotFound(FailureResponse("Account does not exist."));

            return Ok(SuccessResponse($"Account with id '{id}' deleted successfully."));
        }
    }
}
