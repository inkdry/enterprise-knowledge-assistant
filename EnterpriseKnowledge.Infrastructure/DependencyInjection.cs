using EnterpriseKnowledge.Application.Documents;
using EnterpriseKnowledge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseKnowledge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("KnowledgeDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("The KnowledgeDatabase connection string is not configured.");
        }

        services.AddDbContext<KnowledgeDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IDocumentRepository, DocumentRepository>();

        return services;
    }
}
