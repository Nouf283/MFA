using System.Threading.Tasks;

namespace MFA.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string from, string to, string subject, string body);
    }
}