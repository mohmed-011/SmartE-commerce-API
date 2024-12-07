using System.Diagnostics;

namespace SmartE_commerce.MiddleWares
{
    public class ProfilingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProfilingMiddleware> _logger;

        public ProfilingMiddleware(RequestDelegate next , ILogger<ProfilingMiddleware> logger) 
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext Context) 
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await _next(Context);
            stopwatch.Stop();
            _logger.LogInformation($"Requst Took => '{stopwatch.ElapsedMilliseconds}'ms to Execute ");
            
        }
    }
}
