﻿using FinanceManager.Application.Services;
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
        public async Task<IActionResult> GetAccount([FromQuery] Guid id)
        {
            // retrieve user id from claims
            string? userId = GetUserIdOfRequest();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(FailureResponse("User Id is missing in the token."));
            }

            // check if transaction id is provided
            if (id.Equals(Guid.Empty))
                return BadRequest(FailureResponse("Invalid account id."));

            var accountsDomain = await _accountsService.GetAccounts(id, userId);

            if (accountsDomain == null)
                return NotFound(FailureResponse("No account found!"));


            return Ok(SuccessResponse(_domainResponseMapper.Map(accountsDomain)));
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
