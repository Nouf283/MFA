using MFA.Entities;

namespace MFA.Interfaces
{
    public interface ITokenServices
    {
        string CreateToken(AppUser appUser);
    }
}
