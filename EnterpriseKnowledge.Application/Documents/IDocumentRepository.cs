using EnterpriseKnowledge.Domain.Documents;

namespace EnterpriseKnowledge.Application.Documents;

public interface IDocumentRepository
{
    Task<KnowledgeDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(KnowledgeDocument document, CancellationToken cancellationToken = default);
}
