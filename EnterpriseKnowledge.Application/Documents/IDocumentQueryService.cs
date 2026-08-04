namespace EnterpriseKnowledge.Application.Documents;

public interface IDocumentQueryService
{
    Task<DocumentDetailsResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
