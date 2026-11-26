using LibrarySystem.Entities;

namespace LibrarySystem.Services;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user);
}