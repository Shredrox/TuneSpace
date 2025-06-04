using Microsoft.AspNetCore.Identity;
using TuneSpace.Core.Enums;
using TuneSpace.Infrastructure.Identity;

namespace TuneSpace.Infrastructure.Seeding;

public class RoleSeeder(RoleManager<ApplicationRole> roleManager)
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task SeedRolesAsync()
    {
        foreach (var roleName in Enum.GetValues<Roles>())
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName.ToString());
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new ApplicationRole(roleName.ToString()));
            }
        }
    }
}
