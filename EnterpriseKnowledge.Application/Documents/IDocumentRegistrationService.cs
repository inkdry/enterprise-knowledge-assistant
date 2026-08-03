namespace EnterpriseKnowledge.Application.Documents;

public interface IDocumentRegistrationService
{
    Task<RegisterDocumentResult> RegisterAsync(
        RegisterDocumentCommand command,
        CancellationToken cancellationToken = default);
}
