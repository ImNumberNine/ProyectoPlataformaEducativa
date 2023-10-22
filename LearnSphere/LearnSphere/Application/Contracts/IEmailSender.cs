using LearnSphere.Models.ComponentModels;

namespace LearnSphere.Application.Contracts
{
    public interface IEmailSender
    {
        void Send(Email email);
    }
}
