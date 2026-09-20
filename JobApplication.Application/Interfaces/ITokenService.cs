using JobApplication.Domain.Entities;

namespace JobApplication.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user, string role);
    }
}
