namespace FinanceManager.Models.Response
{
    public class Response<T> : BaseResponse
    {
        public T? Data { get; set; }
    }
}
