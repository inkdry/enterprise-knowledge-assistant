using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EnterpriseKnowledge.Api.Tests;

public sealed class DocumentUploadEndpointTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly InMemoryDocumentContentStore _contentStore;

    public DocumentUploadEndpointTests(CustomWebApplicationFactory application)
    {
        _client = application.CreateClient();
        _contentStore = application.ContentStore;
    }

    [Fact]
    public async Task UploadTextDocument_WithValidFile_ReturnsCreated()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var expectedContent = Encoding.UTF8.GetBytes("All systems must use multi-factor authentication.");

        using var request = CreateRequest(expectedContent, "security-policy.txt", "text/plain");

        using var response = await _client.PostAsync(
            "/api/documents/upload",
            request,
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

        using var document = JsonDocument.Parse(responseText);
        var documentId = document.RootElement.GetProperty("id").GetGuid();

        Assert.True(_contentStore.TryGet(documentId, out var storedContent));
        Assert.Equal(expectedContent, storedContent);
    }

    [Fact]
    public async Task UploadDocument_WithUnsupportedType_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var content = Encoding.UTF8.GetBytes("Unsupported content");

        const string wordContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        using var request = CreateRequest(
            content,
            "security-policy.docx",
            wordContentType);

        using var response = await _client.PostAsync(
            "/api/documents/upload",
            request,
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static MultipartFormDataContent CreateRequest(
        byte[] content,
        string fileName,
        string contentType)
    {
        var request = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(content);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        request.Add(fileContent, "file", fileName);

        return request;
    }
}
