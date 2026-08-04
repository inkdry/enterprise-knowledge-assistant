namespace EnterpriseKnowledge.Application.Documents;

public sealed class DocumentQueryService(IDocumentRepository repository)
    : IDocumentQueryService
{
    public async Task<DocumentDetailsResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetByIdAsync(id, cancellationToken);

        if (document is null)
        {
            return null;
        }

        return new DocumentDetailsResult(
            document.Id,
            document.FileName,
            document.ContentType,
            document.SizeInBytes,
            document.Status,
            document.UploadedAtUtc);
    }
}