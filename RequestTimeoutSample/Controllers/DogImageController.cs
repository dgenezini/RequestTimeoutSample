using RequestTimeoutSample.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RequestTimeoutSample.Controllers;

[ApiController]
[Route("[controller]")]
public class DogImageController : ControllerBase
{
    private readonly IDogImageUseCase _dogImageUseCase;
    private readonly ILogger<DogImageController> _logger;

    public DogImageController(IDogImageUseCase dogImageUseCase, ILogger<DogImageController> logger)
    {
        _dogImageUseCase = dogImageUseCase;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _dogImageUseCase.GetRandomDogImage(cancellationToken);
        }
        catch(TaskCanceledException ex)
        {
            _logger.LogError(ex, ex.Message);

            return StatusCode(StatusCodes.Status500InternalServerError, "Request timed out or cancelled");
        }
    }
}
