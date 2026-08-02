namespace EnterpriseKnowledge.Domain.Documents;

public sealed class KnowledgeDocument
{
    private KnowledgeDocument()
    {
    }

    private KnowledgeDocument(
        Guid id,
        string fileName,
        string contentType,
        long sizeInBytes,
        DateTimeOffset uploadedAtUtc)
    {
        Id = id;
        FileName = fileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        UploadedAtUtc = uploadedAtUtc;
        Status = DocumentStatus.Pending;
    }

    public Guid Id { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long SizeInBytes { get; private set; }

    public DateTimeOffset UploadedAtUtc { get; private set; }

    public DocumentStatus Status { get; private set; }

    public static KnowledgeDocument Create(
        string fileName,
        string contentType,
        long sizeInBytes,
        DateTimeOffset uploadedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "A file name is required.",
                nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException(
                "A content type is required.",
                nameof(contentType));
        }

        if (sizeInBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sizeInBytes),
                "The file size must be greater than zero.");
        }

        return new KnowledgeDocument(
            Guid.NewGuid(),
            fileName.Trim(),
            contentType.Trim(),
            sizeInBytes,
            uploadedAtUtc);
    }

    public void MarkAsProcessing()
    {
        TransitionFrom(DocumentStatus.Pending, DocumentStatus.Processing);               
    }

    public void MarkAsReady()
    {
        TransitionFrom(DocumentStatus.Processing, DocumentStatus.Ready);
    }

    public void MarkAsFailed()
    {
        TransitionFrom(DocumentStatus.Processing, DocumentStatus.Failed);
    }

    private void TransitionFrom(DocumentStatus expectedCurrentStatus, DocumentStatus newStatus)
    {
        if (Status != expectedCurrentStatus)
        {
            throw new InvalidOperationException(
                $"A document cannot transition from {Status} to {newStatus}.");
        }

        Status = newStatus;
    }
}
