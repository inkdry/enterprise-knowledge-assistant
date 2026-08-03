using EnterpriseKnowledge.Api.Contracts.Documents;
using EnterpriseKnowledge.Application.Documents;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseKnowledge.Api.Controllers;

[ApiController]
[Route("api/documents")]
public sealed class DocumentsController(
    IDocumentRegistrationService registrationService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<RegisterDocumentResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterDocumentResult>> RegisterAsync(
        [FromBody] RegisterDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterDocumentCommand(request.FileName, request.ContentType, request.SizeInBytes);

        var result = await registrationService.RegisterAsync(command, cancellationToken);

        return Created($"/api/documents/{result.Id}", result);
    }
}
