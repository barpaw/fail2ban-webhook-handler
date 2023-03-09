using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Fail2BanWebhookHandler.Dtos;

namespace Fail2BanWebhookHandler.Services;

public class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiService> _logger;

    public ApiService(ILogger<ApiService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task Notify(EnvsDto envsDto, string message)
    {
        var client = _httpClientFactory.CreateClient("api");

        var matrixNotifierBody = new MatrixNotifier(envsDto.MatrixHomeserverUrl, envsDto.MatrixHomeserverUser,
            envsDto.MatrixHomeserverPasswd, envsDto.MatrixHomeserverRoom, message);

        var jsonString = JsonSerializer.Serialize(matrixNotifierBody);

        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        await client.PostAsync(envsDto.MatrixNotifierUrl, content);
    }

    public async Task<List<MatrixUserCheckResult>?> GetMatrixUsersByIp(string ipAddress, EnvsDto envsDto)
    {

        string jsonStringMatrixUserApi =
            JsonSerializer.Serialize(new MatrixUserCheck(ipAddress, envsDto.MatrixUserCheckApiToken));

        var client = _httpClientFactory.CreateClient("api");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(envsDto.MatrixUserCheckApi),
            Content = new StringContent(jsonStringMatrixUserApi, Encoding.UTF8, MediaTypeNames.Application.Json),
        };

        var response = await client.SendAsync(request).ConfigureAwait(false);

        var encoding = Encoding.ASCII;

        string responseText;

        using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), encoding))
        {
            responseText = reader.ReadToEnd();
        }
        
        var matrixUserCheckResults = JsonSerializer.Deserialize<List<MatrixUserCheckResult>>(responseText);
        
        return matrixUserCheckResults;
    }
}