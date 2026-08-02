using EnterpriseKnowledge.Domain.Documents;

namespace EnterpriseKnowledge.Domain.Tests.Documents;

public sealed class KnowledgeDocumentTests
{
    [Fact]
    public void Create_WithValidValues_CreatesPendingDocument()
    {
        // Arrange
        var uploadedAt = new DateTimeOffset(
            2026, 8, 2, 12, 0, 0, TimeSpan.Zero);

        // Act
        var document = KnowledgeDocument.Create(
            "security-policy.pdf",
            "application/pdf",
            1_024,
            uploadedAt);

        // Assert
        Assert.NotEqual(Guid.Empty, document.Id);
        Assert.Equal("security-policy.pdf", document.FileName);
        Assert.Equal("application/pdf", document.ContentType);
        Assert.Equal(1_024, document.SizeInBytes);
        Assert.Equal(uploadedAt, document.UploadedAtUtc);
        Assert.Equal(DocumentStatus.Pending, document.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithoutFileName_ThrowsArgumentException(
        string fileName)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            KnowledgeDocument.Create(
                fileName,
                "application/pdf",
                1_024,
                DateTimeOffset.UtcNow));

        Assert.Equal("fileName", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithoutContentType_ThrowsArgumentException(
        string contentType)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            KnowledgeDocument.Create(
                "security-policy.pdf",
                contentType,
                1_024,
                DateTimeOffset.UtcNow));

        Assert.Equal("contentType", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidFileSize_ThrowsArgumentOutOfRangeException(
        long sizeInBytes)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            KnowledgeDocument.Create(
                "security-policy.pdf",
                "application/pdf",
                sizeInBytes,
                DateTimeOffset.UtcNow));

        Assert.Equal("sizeInBytes", exception.ParamName);
    }

    [Fact]
    public void StatusTransitions_UpdateDocumentStatus()
    {
        var document = CreateDocument();

        document.MarkAsProcessing();
        Assert.Equal(DocumentStatus.Processing, document.Status);

        document.MarkAsReady();
        Assert.Equal(DocumentStatus.Ready, document.Status);

        document.MarkAsFailed();
        Assert.Equal(DocumentStatus.Failed, document.Status);
    }

    private static KnowledgeDocument CreateDocument()
    {
        return KnowledgeDocument.Create(
            "security-policy.pdf",
            "application/pdf",
            1_024,
            DateTimeOffset.UtcNow);
    }
}
