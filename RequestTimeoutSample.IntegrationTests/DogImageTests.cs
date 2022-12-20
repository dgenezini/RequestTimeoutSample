using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace RequestTimeoutSample.IntegrationTests;

public class DogImageTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DogImageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_DogImage_Returns_200()
    {
        //Arrange
        var wireMockSvr = WireMockServer.Start();

        var factory = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("DogApiUrl", wireMockSvr.Url);
            });

        var httpClient = factory.CreateClient();

        Fixture fixture = new Fixture();

        var responseObj = fixture.Create<Dog>();
        var responseObjJson = JsonSerializer.Serialize(responseObj);

        wireMockSvr
            .Given(Request.Create()
                .WithPath("/breeds/image/random")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(responseObjJson)
                .WithHeader("Content-Type", "application/json")
                .WithStatusCode(HttpStatusCode.OK));

        //Act
        var apiHttpResponse = await httpClient.GetAsync("/DogImage");

        //Assert
        apiHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var dogImageUrl = await apiHttpResponse.Content.ReadAsStringAsync();

        dogImageUrl.Should().BeEquivalentTo(responseObj.message);

        wireMockSvr.Stop();
    }

    [Fact]
    public async Task Timeout_Returns_500WithMessage()
    {
        //Arrange
        var wireMockSvr = WireMockServer.Start();

        var factory = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("DogApiUrl", wireMockSvr.Url);

                //Set the HttpClient timeout to 30
                //so the HttpClient timeout doesn't trigger before
                //our request timeout that is 10 seconds
                builder.UseSetting("HttpClientTimeoutSeconds", "30");

                //Set the Request time-out to 5 seconds for the test to run faster
                builder.UseSetting("RequestTimeoutSeconds", "5");                
            });

        var httpClient = factory.CreateClient();

        Fixture fixture = new Fixture();

        var responseObj = fixture.Create<Dog>();
        var responseObjJson = JsonSerializer.Serialize(responseObj);

        wireMockSvr
            .Given(Request.Create()
                .WithPath("/breeds/image/random")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(responseObjJson)
                .WithHeader("Content-Type", "application/json")
                .WithStatusCode(HttpStatusCode.OK));

        //Add a delay to the response to cause a request timeout in the /DogImage endpoint
        wireMockSvr.AddGlobalProcessingDelay(TimeSpan.FromSeconds(15));

        //Act
        var apiHttpResponse = await httpClient.GetAsync("/DogImage");

        //Assert
        apiHttpResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var responseMessage = await apiHttpResponse.Content.ReadAsStringAsync();

        responseMessage.Should().BeEquivalentTo("Request timed out or cancelled");

        wireMockSvr.Stop();
    }
}