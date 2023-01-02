using System.Threading.Tasks;
using Template.Core.Application.Models.Mail;

namespace Template.Core.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
    }
}
