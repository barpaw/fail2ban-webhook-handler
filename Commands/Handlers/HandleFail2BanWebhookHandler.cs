using Fail2BanWebhookHandler.Services;
using MediatR;

namespace Fail2BanWebhookHandler.Commands.Handlers;

public class HandleFail2BanWebhookHandler : IRequestHandler<HandleFail2BanWebhookCommand, string>
{
    private readonly IApiService _apiService;
    private readonly IEnvironmentService _environmentService;
    private readonly ILogger<HandleFail2BanWebhookHandler> _logger;
    private readonly IMessageService _messageService;

    public HandleFail2BanWebhookHandler(ILogger<HandleFail2BanWebhookHandler> logger,
        IEnvironmentService environmentService, IMessageService messageService, IApiService apiService)
    {
        _logger = logger;
        _environmentService = environmentService;
        _messageService = messageService;
        _apiService = apiService;
    }

    public async Task<string> Handle(HandleFail2BanWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var envsDto = _environmentService.GetEnvDto();
            var message = await _messageService.PrepareMessage(request.RageshakeWebhook, envsDto);
            await _apiService.Notify(envsDto, message);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return "Error";
        }

        return "Ok";
    }
}