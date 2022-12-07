using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Hospitals.Web.Tests;

public class HospitalEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HospitalEndpointTests(WebApplicationFactory<Program> factory)
    {
        this._factory = factory;
    }

    // note: normally for any tests involving data, I'd want to use
    // Respawner to reset the database or use a specific scoping
    // per test so that tests don't interfere with each other.

    // other tests to include:
    // delete with invalid ID
    // put with invalid ID

    [Fact]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/hospital");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task Post_CorrectlySaves_AndReturns_Hospital()
    {
        var hospital = new Hospital(null, "I'm a test hospital");
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/hospital", JsonContent.Create(hospital));
        var result = await response.Content.ReadFromJsonAsync<Hospital>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result.ID);
        Assert.Equal("I'm a test hospital", result.Name);
    }

    [Fact]
    public async Task Delete_ReturnsCorrectStatusCode_And_HospitalIsNoLongerFound()
    {
        var hospital = new Hospital(null, "I'm a test hospital");
        // Arrange
        var client = _factory.CreateClient();

        // Act
        // create and delete hospital
        var response = await client.PostAsync("/hospital", JsonContent.Create(hospital));
        var createdHospital = await response.Content.ReadFromJsonAsync<Hospital>();

        var deleteResponse = await client.DeleteAsync("/hospital/" + createdHospital.ID);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync("/hospital/" + createdHospital.ID);
        var getHospital = await getResponse.Content.ReadFromJsonAsync<Hospital>();

        Assert.Null(getHospital);
    }
}