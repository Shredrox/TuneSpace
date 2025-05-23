﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TuneSpace.Core.Enums;

namespace TuneSpace.Core.Entities;

public class User : IdentityUser<Guid>
{
    [MaxLength(256)]
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenValidity { get; set; }
    public Roles Role { get; set; }
}
