namespace FictiveShop.Core.Responses
{
    public record ApiResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; }

        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMsg(statusCode);
        }

        private string GetDefaultMsg(int statusCode) => statusCode switch
        {
            400 => "Bad request.",
            401 => "Unauthorized.",
            404 => "Not found.",
            500 => "Internal error.",
            _ => null
        };
    }
}