using EnterpriseKnowledge.Domain.Documents;

namespace EnterpriseKnowledge.Application.Documents;

public sealed class DocumentRegistrationService(
    IDocumentRepository repository,
    TimeProvider timeProvider) : IDocumentRegistrationService
{
    public async Task<RegisterDocumentResult> RegisterAsync(
        RegisterDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var document = KnowledgeDocument.Create(
            command.FileName,
            command.ContentType,
            command.SizeInBytes,
            timeProvider.GetUtcNow());

        await repository.AddAsync(document, cancellationToken);

        return new RegisterDocumentResult(
            document.Id,
            document.FileName,
            document.Status,
            document.UploadedAtUtc);
    }
}
