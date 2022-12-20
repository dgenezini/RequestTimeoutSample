using Refit;

namespace RequestTimeoutSample;

public interface IDogApi
{
    [Get("/breeds/image/random")]
    Task<Dog> GetRandomDog(CancellationToken cancellationToken);
}