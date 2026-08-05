using EnterpriseKnowledge.Domain.Documents;

namespace EnterpriseKnowledge.Application.Documents;

public sealed class DocumentUploadService(
    IDocumentRepository repository,
    IDocumentContentStore contentStore,
    TimeProvider timeProvider) : IDocumentUploadService
{
    private const long MaximumFileSize = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "text/plain"
        };

    public async Task<RegisterDocumentResult> UploadAsync(
        UploadDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        Validate(command);

        // Keep only the final file name supplied by the client.
        var safeFileName = Path.GetFileName(command.FileName);

        var document = KnowledgeDocument.Create(
            safeFileName,
            command.ContentType,
            command.SizeInBytes,
            timeProvider.GetUtcNow());

        try
        {
            // Store content before committing its database record.
            await contentStore.SaveAsync(
                document.Id,
                command.Content,
                cancellationToken);

            await repository.AddAsync(document, cancellationToken);
        }
        catch
        {
            // Remove partially stored content when registration fails.
            await contentStore.DeleteAsync(
                document.Id,
                CancellationToken.None);

            throw;
        }

        return new RegisterDocumentResult(
            document.Id,
            document.FileName,
            document.Status,
            document.UploadedAtUtc);
    }

    private static void Validate(UploadDocumentCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.FileName))
        {
            throw new ArgumentException(
                "A file name is required.",
                nameof(command));
        }

        if (!AllowedContentTypes.Contains(command.ContentType))
        {
            throw new ArgumentException(
                "Only PDF and plain-text documents are supported.",
                nameof(command));
        }

        if (command.SizeInBytes is <= 0 or > MaximumFileSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(command),
                "The file must be between 1 byte and 10 MB.");
        }

        if (!command.Content.CanRead)
        {
            throw new ArgumentException(
                "The document content must be readable.",
                nameof(command));
        }
    }
}
