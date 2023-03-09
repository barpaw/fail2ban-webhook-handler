using Fail2BanWebhookHandler.Dtos;

namespace Fail2BanWebhookHandler.Services;

public interface IEnvironmentService
{
    EnvsDto GetEnvDto();
}