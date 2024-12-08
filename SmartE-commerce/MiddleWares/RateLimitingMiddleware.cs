namespace SmartE_commerce.MiddleWares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        private static int _counter = 0;
        private static DateTime _lastRequestDate = DateTime.Now;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;

        }
        public async Task Invoke(HttpContext Context)
        {
            _counter++;
            if (DateTime.Now.Subtract(_lastRequestDate).Seconds > 10)
            {
                _counter = 1;
                _lastRequestDate = DateTime.Now;
                await _next(Context);
            }
            else
            {
                if (_counter > 20)
                {
                    _lastRequestDate = DateTime.Now;
                    Context.Response.WriteAsync("Rate");
                }
                else
                {
                    _lastRequestDate = DateTime.Now;
                    await _next(Context);
                }
            }

        }
    }
}
