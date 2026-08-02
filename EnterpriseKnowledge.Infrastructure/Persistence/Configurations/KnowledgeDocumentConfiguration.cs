using EnterpriseKnowledge.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseKnowledge.Infrastructure.Persistence.Configurations;

public sealed class KnowledgeDocumentConfiguration
    : IEntityTypeConfiguration<KnowledgeDocument>
{
    public void Configure(EntityTypeBuilder<KnowledgeDocument> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(document => document.Id);

        builder.Property(document => document.FileName).HasMaxLength(255).IsRequired();

        builder.Property(document => document.ContentType).HasMaxLength(100).IsRequired();

        builder.Property(document => document.SizeInBytes).IsRequired();

        builder.Property(document => document.UploadedAtUtc).IsRequired();

        builder.Property(document => document.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
    }
}
