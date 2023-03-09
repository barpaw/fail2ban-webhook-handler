using MediatR;

namespace Fail2BanWebhookHandler.Commands;

public class HandleFail2BanWebhookCommand : IRequest<string>
{
    public HandleFail2BanWebhookCommand(Fail2BanWebhook rageshakeWebhook)
    {
        RageshakeWebhook = rageshakeWebhook;
    }

    public Fail2BanWebhook RageshakeWebhook { get; set; }
}