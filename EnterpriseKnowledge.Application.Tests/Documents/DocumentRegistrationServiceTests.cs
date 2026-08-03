using EnterpriseKnowledge.Application.Documents;
using EnterpriseKnowledge.Domain.Documents;

namespace EnterpriseKnowledge.Application.Tests.Documents;

public sealed class DocumentRegistrationServiceTests
{
    [Fact]
    public async Task RegisterAsync_WithValidCommand_PersistsPendingDocument()
    {
        var uploadedAt = new DateTimeOffset(2026, 8, 2, 12, 0, 0, TimeSpan.Zero);

        var repository = new RecordingDocumentRepository();
        var timeProvider = new FixedTimeProvider(uploadedAt);
        var service = new DocumentRegistrationService(repository, timeProvider);

        var command = new RegisterDocumentCommand(
            "security-policy.pdf",
            "application/pdf",
            1_024);

        var result = await service.RegisterAsync(command);

        Assert.NotNull(repository.AddedDocument);
        Assert.Equal(repository.AddedDocument.Id, result.Id);
        Assert.Equal("security-policy.pdf", result.FileName);
        Assert.Equal(DocumentStatus.Pending, result.Status);
        Assert.Equal(uploadedAt, result.UploadedAtUtc);
    }

    [Fact]
    public async Task RegisterAsync_WithNullCommand_ThrowsArgumentNullException()
    {
        var repository = new RecordingDocumentRepository();
        var service = new DocumentRegistrationService(
            repository,
            TimeProvider.System);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.RegisterAsync(null!));
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class RecordingDocumentRepository : IDocumentRepository
    {
        public KnowledgeDocument? AddedDocument { get; private set; }

        public Task<KnowledgeDocument?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<KnowledgeDocument?>(null);
        }

        public Task AddAsync(
            KnowledgeDocument document,
            CancellationToken cancellationToken = default)
        {
            AddedDocument = document;
            return Task.CompletedTask;
        }
    }
}
