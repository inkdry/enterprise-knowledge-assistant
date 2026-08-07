using EnterpriseKnowledge.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using EnterpriseKnowledge.Application.Documents;

namespace EnterpriseKnowledge.Api.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"EnterpriseKnowledgeTests-{Guid.NewGuid()}";
    public InMemoryDocumentContentStore ContentStore { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<KnowledgeDbContext>>();

            services.RemoveAll<IDbContextOptionsConfiguration<KnowledgeDbContext>>();

            services.AddDbContext<KnowledgeDbContext>(options =>options.UseInMemoryDatabase(_databaseName));

            // Replace physical storage with isolated test storage.
            services.RemoveAll<IDocumentContentStore>();
            services.AddSingleton<IDocumentContentStore>(ContentStore);
        });
    }
}
