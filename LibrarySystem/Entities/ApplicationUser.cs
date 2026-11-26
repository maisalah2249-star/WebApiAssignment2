using Microsoft.AspNetCore.Identity;

namespace LibrarySystem.Entities;

/// <summary>
/// Application user with extended profile information.
/// Inherits from IdentityUser to support ASP.NET Core Identity features.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}