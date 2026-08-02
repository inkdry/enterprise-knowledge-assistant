using EnterpriseKnowledge.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseKnowledge.Infrastructure.Persistence;

public sealed class KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options)
    : DbContext(options)
{
    public DbSet<KnowledgeDocument> Documents => Set<KnowledgeDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KnowledgeDbContext).Assembly);
    }
}
