using EnterpriseKnowledge.Domain.Documents;

namespace EnterpriseKnowledge.Application.Documents;

public sealed record DocumentDetailsResult(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeInBytes,
    DocumentStatus Status,
    DateTimeOffset UploadedAtUtc);
