namespace EnterpriseKnowledge.Application.Documents;

public interface IDocumentContentStore
{
    Task SaveAsync(
        Guid documentId,
        Stream content,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);
}
