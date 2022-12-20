namespace RequestTimeoutSample.Middleware;

public class TimeoutCancellationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TimeSpan _timeout;

    public TimeoutCancellationMiddleware(RequestDelegate next, TimeoutCancellationMiddlewareOptions options)
    {
        _next = next;
        _timeout = options.Timeout;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();
        timeoutCancellationTokenSource.CancelAfter(_timeout);

        using var combinedCancellationTokenSource =
            CancellationTokenSource
                .CreateLinkedTokenSource(timeoutCancellationTokenSource.Token, context.RequestAborted);

        context.RequestAborted = combinedCancellationTokenSource.Token;

        await _next(context);
    }
}
