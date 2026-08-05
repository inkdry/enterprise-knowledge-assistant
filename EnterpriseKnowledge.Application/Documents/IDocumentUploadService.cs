namespace EnterpriseKnowledge.Application.Documents;

public interface IDocumentUploadService
{
    Task<RegisterDocumentResult> UploadAsync(
        UploadDocumentCommand command,
        CancellationToken cancellationToken = default);
}
