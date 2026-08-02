using EnterpriseKnowledge.Application.Documents;
using EnterpriseKnowledge.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseKnowledge.Infrastructure.Persistence;

public sealed class DocumentRepository(KnowledgeDbContext dbContext)
    : IDocumentRepository
{
    public Task<KnowledgeDocument?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Documents.SingleOrDefaultAsync(
            document => document.Id == id,
            cancellationToken);
    }

    public async Task AddAsync(
        KnowledgeDocument document,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        await dbContext.Documents.AddAsync(document, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
