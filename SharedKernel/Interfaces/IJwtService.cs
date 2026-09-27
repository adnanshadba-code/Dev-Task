
using SharedKernel.Classes;

namespace SharedKernel.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user, List<string> permissions);
    }
}
