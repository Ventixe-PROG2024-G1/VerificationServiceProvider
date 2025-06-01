namespace VerificationServiceProvider.Middlewares
{
    public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
            {
                context.Response.StatusCode = 401;
                return;
            }

            var validApiKey = _configuration.GetValue<string>("SecretKeys:AuthenticationKey");

            if (!validApiKey.Equals(apiKey))
            {
                context.Response.StatusCode = 403;
                return;
            }

            await _next(context);
        }
    }
}