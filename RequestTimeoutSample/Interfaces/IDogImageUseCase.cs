namespace RequestTimeoutSample.Interfaces;

public interface IDogImageUseCase
{
    Task<string> GetRandomDogImage(CancellationToken cancellationToken);
}