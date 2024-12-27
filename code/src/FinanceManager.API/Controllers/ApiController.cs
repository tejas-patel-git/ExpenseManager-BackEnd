using FinanceManager.Models.Response;
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

        protected Response<T> PrepareSuccessResponse<T>(T responseData)
        {
            return new()
            {
                Data = responseData,
                Success = true
            };
        }
    }
}
