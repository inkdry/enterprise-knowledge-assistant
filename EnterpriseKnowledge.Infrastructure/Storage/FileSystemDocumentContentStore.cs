using EnterpriseKnowledge.Application.Documents;

namespace EnterpriseKnowledge.Infrastructure.Storage;

public sealed class FileSystemDocumentContentStore : IDocumentContentStore
{
    private readonly string _rootPath;

    public FileSystemDocumentContentStore(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("A document storage path is required.", nameof(rootPath));
        }

        _rootPath = Path.GetFullPath(rootPath);
        Directory.CreateDirectory(_rootPath);
    }

    public async Task SaveAsync(Guid documentId, Stream content, CancellationToken cancellationToken = default)
    {
        ValidateDocumentId(documentId);
        ArgumentNullException.ThrowIfNull(content);

        if (!content.CanRead)
        {
            throw new ArgumentException("The content stream must be readable.", nameof(content));
        }

        var finalPath = GetDocumentPath(documentId);
        var temporaryPath = GetTemporaryPath(documentId);

        try
        {
            // Close the temporary file before moving it.
            await using (var output = new FileStream(
                temporaryPath,
                new FileStreamOptions
                {
                    Mode = FileMode.CreateNew,
                    Access = FileAccess.Write,
                    Share = FileShare.None,
                    BufferSize = 81_920,
                    Options = FileOptions.Asynchronous
                        | FileOptions.SequentialScan
                }))
            {
                await content.CopyToAsync(output, cancellationToken);
                await output.FlushAsync(cancellationToken);
            }

            // Publish the completed file after its handle is closed.
            File.Move(temporaryPath, finalPath);
        }
        catch
        {
            DeleteFileIfPresent(temporaryPath);
            throw;
        }
    }

    public Task DeleteAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        ValidateDocumentId(documentId);
        cancellationToken.ThrowIfCancellationRequested();

        DeleteFileIfPresent(GetDocumentPath(documentId));

        return Task.CompletedTask;
    }

    private string GetDocumentPath(Guid documentId)
    {
        return Path.Combine(_rootPath, $"{documentId:N}.bin");
    }

    private string GetTemporaryPath(Guid documentId)
    {
        var suffix = Guid.NewGuid().ToString("N");

        return Path.Combine(_rootPath, $"{documentId:N}.{suffix}.tmp");
    }

    private static void ValidateDocumentId(Guid documentId)
    {
        if (documentId == Guid.Empty)
        {
            throw new ArgumentException("A document ID is required.", nameof(documentId));
        }
    }

    private static void DeleteFileIfPresent(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
