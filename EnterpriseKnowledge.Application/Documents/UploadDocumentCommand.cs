namespace EnterpriseKnowledge.Application.Documents;

public sealed record UploadDocumentCommand(
    string FileName,
    string ContentType,
    long SizeInBytes,
    Stream Content);