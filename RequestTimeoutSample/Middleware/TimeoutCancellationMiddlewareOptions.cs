namespace RequestTimeoutSample.Middleware;

public class TimeoutCancellationMiddlewareOptions
{
    public TimeSpan Timeout { get; set; }

    public TimeoutCancellationMiddlewareOptions(TimeSpan timeout)
    {
        Timeout = timeout;
    }
}