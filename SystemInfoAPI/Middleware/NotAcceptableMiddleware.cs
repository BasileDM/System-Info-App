namespace SystemInfoApi.Middleware
{
    public class NotAcceptableMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context) {
            var acceptHeader = context.Request.Headers.Accept;
            if (context.Request.Headers.Accept != "application/json") {
                context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                await context.Response.WriteAsync(
                    $"The Requested Format {acceptHeader} is Not Supported.");
                return;
            }
            await _next(context);
        }
    }
}