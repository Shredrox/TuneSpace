using Microsoft.AspNetCore.Http;

namespace TuneSpace.Core.DTOs.Requests.Band;

public record CreateBandRequest(
    string Name,
    string Description,
    string Genre,
    string Location,
    IFormFile Picture);