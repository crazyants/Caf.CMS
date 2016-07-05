
namespace CAF.Infrastructure.Core.Domain.Users
{ 
    public enum PasswordFormat : int
    {
        Clear = 0,
        Hashed = 1,
        Encrypted = 2
    }
}
