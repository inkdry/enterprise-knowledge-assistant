using System.ComponentModel.DataAnnotations;

namespace EnterpriseKnowledge.Api.Contracts.Documents;

public sealed record RegisterDocumentRequest
{
    [Required(ErrorMessage = "File name is required.")]
    [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
    public required string FileName { get; init; }

    [Required(ErrorMessage = "Content type is required.")]
    [StringLength(100, ErrorMessage = "Content type cannot exceed 100 characters.")]
    public required string ContentType { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than zero.")]
    public long SizeInBytes { get; init; }
}
