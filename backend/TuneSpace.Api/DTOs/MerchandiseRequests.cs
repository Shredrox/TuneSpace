using System.ComponentModel.DataAnnotations;

namespace TuneSpace.Api.DTOs;

public class MerchandiseCreateRequest
{
    [Required]
    public string BandId { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public required string Price { get; set; }

    public IFormFile? Image { get; set; }
}

public class MerchandiseUpdateRequest
{
    [Required]
    public string Id { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Price { get; set; }

    public IFormFile? Image { get; set; }
}
