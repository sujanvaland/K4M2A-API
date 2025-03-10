using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiritualNetwork.API.Middleware
{
    public class PrerenderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PrerenderMiddleware> _logger;
        private readonly string _prerenderServiceUrl = "https://service.prerender.io/";
        private readonly string _prerenderToken;

        public PrerenderMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<PrerenderMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _prerenderToken = configuration["Prerender:Token"];
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsBotRequest(context.Request))
            {
                string prerenderedUrl = $"{_prerenderServiceUrl}{context.Request.GetEncodedUrl()}";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Prerender-Token", _prerenderToken);

                _logger.LogInformation($"Fetching prerendered page from: {prerenderedUrl}");

                var response = await client.GetAsync(prerenderedUrl);
                if (response.IsSuccessStatusCode)
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
                    return;
                }
            }

            await _next(context);
        }

        private bool IsBotRequest(HttpRequest request)
        {
            string userAgent = request.Headers["User-Agent"].ToString();
            if (string.IsNullOrEmpty(userAgent)) return false;

            string[] botKeywords = { "googlebot", "bingbot", "yandex", "baiduspider", "facebookexternalhit", "twitterbot" };
            return botKeywords.Any(bot => userAgent.ToLower().Contains(bot));
        }
    }

}
