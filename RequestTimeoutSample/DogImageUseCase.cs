namespace RequestTimeoutSample.Interfaces;

public class DogImageUseCase: IDogImageUseCase
{
    private readonly IDogApi _dogApi;

    public DogImageUseCase(IDogApi dogApi)
    {
        _dogApi = dogApi;
    }

    public async Task<string> GetRandomDogImage(CancellationToken cancellationToken)
    {
        var dog = await _dogApi.GetRandomDog(cancellationToken);

        return dog.message;
    }
}