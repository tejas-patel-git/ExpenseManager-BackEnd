﻿using FinanceManager.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ProducesResponseType(401)]
    public class ApiController : ControllerBase
    {
        public ApiController()
        {

        }

        protected Response<T> PrepareResponse<T>(T responseData, bool isSuccess = true)
        {
            return new()
            {
                Data = responseData,
                Success = isSuccess
            };
        }

        protected Response PrepareResponse(string errorMessage)
        {
            return new()
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
