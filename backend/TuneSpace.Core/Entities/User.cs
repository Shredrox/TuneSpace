using Microsoft.AspNetCore.Identity;
using TuneSpace.Core.Enums;

namespace TuneSpace.Core.Entities;

public class User : IdentityUser<Guid>
{
    public Roles Role { get; set; }
    public byte[]? ProfilePicture { get; set; }
}
