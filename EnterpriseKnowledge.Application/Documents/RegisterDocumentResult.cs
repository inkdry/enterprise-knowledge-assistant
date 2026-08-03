using EnterpriseKnowledge.Domain.Documents;

namespace EnterpriseKnowledge.Application.Documents;

public sealed record RegisterDocumentResult(
    Guid Id,
    string FileName,
    DocumentStatus Status,
    DateTimeOffset UploadedAtUtc);
