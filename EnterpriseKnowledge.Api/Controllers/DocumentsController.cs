using EnterpriseKnowledge.Api.Contracts.Documents;
using EnterpriseKnowledge.Application.Documents;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseKnowledge.Api.Controllers;

[ApiController]
[Route("api/documents")]
public sealed class DocumentsController(
    IDocumentRegistrationService registrationService,
    IDocumentQueryService queryService,
    IDocumentUploadService uploadService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<RegisterDocumentResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
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

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(11 * 1024 * 1024)]
    [ProducesResponseType<RegisterDocumentResult>(
    StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(
    StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    public async Task<ActionResult<RegisterDocumentResult>> UploadAsync(
    [FromForm] IFormFile file,
    CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError("file", "A non-empty document is required.");

            return ValidationProblem(ModelState);
        }

        await using var content = file.OpenReadStream();

        var command = new UploadDocumentCommand(file.FileName, file.ContentType, file.Length, content);

        try
        {
            // Validate and store the uploaded content.
            var result = await uploadService.UploadAsync(command, cancellationToken);

            return CreatedAtRoute("GetDocumentById", new { id = result.Id }, result);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError("file", exception.Message);

            return ValidationProblem(ModelState);
        }
    }

    [HttpGet("{id:guid}", Name = "GetDocumentById")]
    [ProducesResponseType<DocumentDetailsResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public async Task<ActionResult<DocumentDetailsResult>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await queryService.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}