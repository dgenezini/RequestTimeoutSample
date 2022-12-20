namespace RequestTimeoutSample.Middleware;

public static class TimeoutCancellationMiddlewareExtensions
{
    public static IApplicationBuilder UseTimeoutCancellationToken(
        this IApplicationBuilder builder, TimeSpan timeout)
    {
        return builder.UseMiddleware<TimeoutCancellationMiddleware>(
            new TimeoutCancellationMiddlewareOptions(timeout));
    }
}
