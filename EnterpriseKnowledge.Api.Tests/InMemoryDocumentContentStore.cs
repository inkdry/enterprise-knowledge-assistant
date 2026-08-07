using System.Collections.Concurrent;
using EnterpriseKnowledge.Application.Documents;

namespace EnterpriseKnowledge.Api.Tests;

public sealed class InMemoryDocumentContentStore : IDocumentContentStore
{
    private readonly ConcurrentDictionary<Guid, byte[]> _documents = new();

    public async Task SaveAsync(
        Guid documentId,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        // Copy content so the test does not retain the request stream.
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);

        if (!_documents.TryAdd(documentId, buffer.ToArray()))
        {
            throw new InvalidOperationException("The document already exists.");
        }
    }

    public Task DeleteAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _documents.TryRemove(documentId, out _);

        return Task.CompletedTask;
    }

    public bool TryGet(Guid documentId, out byte[]? content)
    {
        return _documents.TryGetValue(documentId, out content);
    }
}