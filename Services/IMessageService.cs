using Fail2BanWebhookHandler.Dtos;

namespace Fail2BanWebhookHandler.Services;

public interface IMessageService
{
    Task<string> PrepareMessage(Fail2BanWebhook fail2BanWebhook, EnvsDto envsDto);
}