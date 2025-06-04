namespace TuneSpace.Api.DTOs;

public record ProfileUpdateRequest(
    string Username,
    IFormFile? File
);
