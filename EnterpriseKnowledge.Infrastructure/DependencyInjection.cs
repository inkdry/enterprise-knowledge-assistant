using EnterpriseKnowledge.Application.Documents;
using EnterpriseKnowledge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EnterpriseKnowledge.Infrastructure.Storage;

namespace EnterpriseKnowledge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration,
    string contentRootPath)
    {
        var connectionString = configuration.GetConnectionString("KnowledgeDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("The KnowledgeDatabase connection string is not configured.");
        }

        services.AddDbContext<KnowledgeDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IDocumentRepository, DocumentRepository>();

        var configuredPath = configuration["DocumentStorage:RootPath"];

        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            throw new InvalidOperationException("The document storage path is not configured.");
        }

        // Resolve relative storage beneath the API project.
        var storagePath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(contentRootPath, configuredPath);

        services.AddSingleton<IDocumentContentStore>(_ => new FileSystemDocumentContentStore(storagePath));

        return services;
    }
}
