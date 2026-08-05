using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnterpriseKnowledge.Api.Tests;

public sealed class DocumentsEndpointTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DocumentsEndpointTests(CustomWebApplicationFactory application)
    {
        _client = application.CreateClient();
    }

    [Fact]
    public async Task RegisterDocument_WithValidRequest_ReturnsCreated()
    {
        var request = new
        {
            fileName = "security-policy.pdf",
            contentType = "application/pdf",
            sizeInBytes = 1_024
        };

        using var response = await _client.PostAsJsonAsync("/api/documents", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);
        var root = document.RootElement;

        Assert.NotEqual(Guid.Empty, root.GetProperty("id").GetGuid());

        Assert.Equal(
            "security-policy.pdf",
            root.GetProperty("fileName").GetString());

        Assert.Equal(
            "Pending",
            root.GetProperty("status").GetString());

        using var getResponse = await _client.GetAsync(response.Headers.Location!);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getContent = await getResponse.Content.ReadAsStringAsync();
        using var retrievedDocument = JsonDocument.Parse(getContent);
        var retrievedRoot = retrievedDocument.RootElement;

        Assert.Equal(root.GetProperty("id").GetGuid(), retrievedRoot.GetProperty("id").GetGuid());

        Assert.Equal("security-policy.pdf", retrievedRoot.GetProperty("fileName").GetString());

        Assert.Equal("application/pdf", retrievedRoot.GetProperty("contentType").GetString());

        Assert.Equal(1_024, retrievedRoot.GetProperty("sizeInBytes").GetInt64());

        Assert.Equal("Pending", retrievedRoot.GetProperty("status").GetString());
    }

    [Fact]
    public async Task RegisterDocument_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = new
        {
            fileName = string.Empty,
            contentType = string.Empty,
            sizeInBytes = 0
        };

        using var response = await _client.PostAsJsonAsync("/api/documents", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);
        var errors = document.RootElement.GetProperty("errors");

        Assert.True(errors.TryGetProperty("FileName", out _));
        Assert.True(errors.TryGetProperty("ContentType", out _));
        Assert.True(errors.TryGetProperty("SizeInBytes", out _));
    }

    [Fact]
    public async Task GetDocument_WithUnknownId_ReturnsNotFound()
    {
        var unknownId = Guid.NewGuid();

        using var response = await _client.GetAsync(
            $"/api/documents/{unknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
