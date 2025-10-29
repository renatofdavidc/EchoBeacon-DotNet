using System.Security.Claims;

namespace ProjetoChallengeMottu.Middleware
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER_NAME = "X-Api-Key";

        public ApiKeyAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            // Permite acesso a swagger e health checks sem autenticação
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.StartsWith("/swagger") || 
                path.StartsWith("/health") || 
                path == "/")
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key não fornecida");
                return;
            }

            var apiKey = configuration.GetValue<string>("ApiKey");

            if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key inválida");
                return;
            }

            // Define a identidade do usuário autenticado
            var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
            var identity = new ClaimsIdentity(claims, "ApiKey");
            context.User = new ClaimsPrincipal(identity);

            await _next(context);
        }
    }

    public static class ApiKeyAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAuthenticationMiddleware>();
        }
    }
}
