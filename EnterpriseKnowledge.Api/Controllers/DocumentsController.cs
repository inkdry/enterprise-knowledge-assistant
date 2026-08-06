using EnterpriseKnowledge.Api.Contracts.Documents;
using EnterpriseKnowledge.Application.Documents;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseKnowledge.Api.Controllers;

[ApiController]
[Route("api/documents")]
public sealed class DocumentsController(
    IDocumentRegistrationService registrationService,
    IDocumentQueryService queryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<RegisterDocumentResult>(
        StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterDocumentResult>> RegisterAsync(
        [FromBody] RegisterDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterDocumentCommand(
            request.FileName,
            request.ContentType,
            request.SizeInBytes);

        var result = await registrationService.RegisterAsync(
            command,
            cancellationToken);

        return CreatedAtRoute("GetDocumentById", new { id = result.Id }, result);
    }


    [HttpGet("{id:guid}", Name = "GetDocumentById")]
    [ProducesResponseType<DocumentDetailsResult>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public async Task<ActionResult<DocumentDetailsResult>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await queryService.GetByIdAsync(
            id,
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}