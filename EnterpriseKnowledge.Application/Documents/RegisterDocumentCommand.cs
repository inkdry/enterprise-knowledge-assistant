namespace EnterpriseKnowledge.Application.Documents;

public sealed record RegisterDocumentCommand(
    string FileName,
    string ContentType,
    long SizeInBytes);
