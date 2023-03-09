using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Fail2BanWebhookHandler.Dtos;

namespace Fail2BanWebhookHandler.Services;

public class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IApiService _apiService;

    public MessageService(ILogger<MessageService> logger, IApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task<string> PrepareMessage(Fail2BanWebhook fail2BanWebhook, EnvsDto envsDto)
    {
        StringBuilder sb = new StringBuilder();

        List<MatrixUserCheckResult>? matrixUserCheckResults = new List<MatrixUserCheckResult>();

        string? ipAddress = null;

        try
        {
            System.Text.RegularExpressions.Regex ip =
                new System.Text.RegularExpressions.Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            System.Text.RegularExpressions.MatchCollection result = ip.Matches(fail2BanWebhook.Message);

            ipAddress = result[0].ToString();
        }
        catch (Exception ex)
        {
            ipAddress = null;
            _logger.LogError("Can't parse ip address {ex}", ex.Message);
        }

        if ((fail2BanWebhook.Message.Contains("**BANNED**") || fail2BanWebhook.Message.Contains("**UNBANNED**")) &&
            ipAddress is not null)
        {
            matrixUserCheckResults = await _apiService.GetMatrixUsersByIp(ipAddress, envsDto);
        }

        sb.Append(envsDto.MatrixNotifierMessageHeader);
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append(fail2BanWebhook.Message);
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);

        if (ipAddress is not null)
        {
            sb.Append($"https://www.abuseipdb.com/check/{ipAddress}");
            sb.Append(Environment.NewLine);
            sb.Append($"https://www.shodan.io/host/{ipAddress}");
            sb.Append(Environment.NewLine);
            sb.Append($"https://search.censys.io/hosts/{ipAddress}");
            sb.Append(Environment.NewLine);
            sb.Append($"https://db-ip.com/{ipAddress}");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);


            if (matrixUserCheckResults is not null &&
                matrixUserCheckResults.Any() &&
                (fail2BanWebhook.Message.Contains("**BANNED**") ||
                 (fail2BanWebhook.Message.Contains("**UNBANNED**"))) &&
                !fail2BanWebhook.Message.Contains("jail has started") &&
                !fail2BanWebhook.Message.Contains("jail has stopped"))
            {
                sb.Append($"Matrix Users associated with this ip address:");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                foreach (var matrixUserCheckResult in matrixUserCheckResults)
                {
                    sb.Append($"User: {matrixUserCheckResult.UserId}");
                    sb.Append(Environment.NewLine);
                    sb.Append($"UserAgent: {matrixUserCheckResult.UserAgent}");
                    sb.Append(Environment.NewLine);
                    sb.Append($"LastSeen: {matrixUserCheckResult.LastSeen}");
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append("- - -");
                    sb.Append(Environment.NewLine);
                    sb.Append(envsDto.MatrixMessageFooter);
                }
            }
            else
            {
                sb.Append("- - -");
                sb.Append(Environment.NewLine);
                sb.Append($"Not Found Matrix User associated with this ip address.");
                sb.Append(Environment.NewLine);
            }
        }
        
        string message = sb.ToString();

        return message;
    }
}