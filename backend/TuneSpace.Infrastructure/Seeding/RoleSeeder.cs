using Microsoft.AspNetCore.Identity;
using TuneSpace.Core.Enums;
using TuneSpace.Infrastructure.Identity;

namespace TuneSpace.Infrastructure.Seeding;

public class RoleSeeder(RoleManager<ApplicationRole> roleManager)
{
    private static readonly SemaphoreSlim _seedingLock = new(1, 1);

    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private static bool _rolesSeeded = false;

    public async Task SeedRolesAsync()
    {
        if (_rolesSeeded)
        {
            return;
        }

        await _seedingLock.WaitAsync();
        try
        {
            if (_rolesSeeded)
            {
                return;
            }

            foreach (var roleName in Enum.GetValues<Roles>())
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName.ToString());
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new ApplicationRole(roleName.ToString()));
                }
            }

            _rolesSeeded = true;
        }
        finally
        {
            _seedingLock.Release();
        }
    }
}
