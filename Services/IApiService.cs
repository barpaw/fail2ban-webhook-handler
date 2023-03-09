using Fail2BanWebhookHandler.Dtos;

namespace Fail2BanWebhookHandler.Services;

public interface IApiService
{
    Task Notify(EnvsDto envsDto, string message);
    Task<List<MatrixUserCheckResult>?> GetMatrixUsersByIp(string ipAddress, EnvsDto envsDto);
}