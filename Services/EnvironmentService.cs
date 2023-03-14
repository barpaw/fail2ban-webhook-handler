using Fail2BanWebhookHandler.Dtos;

namespace Fail2BanWebhookHandler.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly ILogger<EnvironmentService> _logger;

    public EnvironmentService(ILogger<EnvironmentService> logger)
    {
        _logger = logger;
    }

    public EnvsDto GetEnvDto()
    {
        var matrixUserCheckApi = Environment.GetEnvironmentVariable("MATRIX_USER_CHECK_API");
        var matrixUserCheckApiToken =
            Environment.GetEnvironmentVariable("MATRIX_USER_CHECK_API_TOKEN");
        var matrixNotifierUrl =
            Environment.GetEnvironmentVariable("MATRIX_NOTIFIER_URL");
        var matrixNotifierMessageHeader =
            Environment.GetEnvironmentVariable("MATRIX_NOTIFIER_MESSAGE_HEADER");
        var matrixHomeserverUrl =
            Environment.GetEnvironmentVariable("MATRIX_HOMESERVER_URL");
        var matrixHomeserverUser =
            Environment.GetEnvironmentVariable("MATRIX_HOMESERVER_USER");
        var matrixHomeserverPasswd =
            Environment.GetEnvironmentVariable("MATRIX_HOMESERVER_PASSWD");
        var matrixHomeserverRoom =
            Environment.GetEnvironmentVariable("MATRIX_HOMESERVER_ROOM");
        var matrixMessageFooter =
            Environment.GetEnvironmentVariable("MATRIX_MESSAGE_FOOTER");

        var matrixUserCheckApiCondition = matrixUserCheckApi is not null && matrixUserCheckApi.Length > 0;
        var matrixUserCheckApiTokenCondition =
            matrixUserCheckApiToken is not null && matrixUserCheckApiToken.Length > 0;
        var matrixNotifierUrlCondition = matrixNotifierUrl is not null && matrixNotifierUrl.Length > 0;
        var matrixNotifierMessageHeaderCondition =
            matrixNotifierMessageHeader is not null && matrixNotifierMessageHeader.Length > 0;
        var matrixHomeserverUrlCondition = matrixHomeserverUrl is not null && matrixHomeserverUrl.Length > 0;
        var matrixHomeserverUserCondition = matrixHomeserverUser is not null && matrixHomeserverUser.Length > 0;
        var matrixHomeserverPasswdCondition = matrixHomeserverPasswd is not null && matrixHomeserverPasswd.Length > 0;
        var matrixHomeserverRoomCondition =
            matrixHomeserverRoom is not null && matrixHomeserverRoom.Length > 0;
        var matrixMessageFooterCondition =
            matrixMessageFooter is not null;

        if (matrixUserCheckApiCondition && matrixUserCheckApiTokenCondition && matrixHomeserverUserCondition &&
            matrixHomeserverPasswdCondition && matrixHomeserverUrlCondition && matrixHomeserverRoomCondition &&
            matrixNotifierUrlCondition && matrixNotifierMessageHeaderCondition && matrixMessageFooterCondition)
        {
            var envsDto = new EnvsDto();

            envsDto.MatrixUserCheckApi = matrixUserCheckApi;
            envsDto.MatrixUserCheckApiToken = matrixUserCheckApiToken;
            envsDto.MatrixHomeserverUser = matrixHomeserverUser;
            envsDto.MatrixHomeserverPasswd = matrixHomeserverPasswd;
            envsDto.MatrixHomeserverUrl = matrixHomeserverUrl;
            envsDto.MatrixHomeserverRoom = matrixHomeserverRoom;
            envsDto.MatrixNotifierUrl = matrixNotifierUrl;
            envsDto.MatrixNotifierMessageHeader = matrixNotifierMessageHeader;
            envsDto.MatrixMessageFooter = matrixMessageFooter;

            return envsDto;
        }
        else
        {
            throw new Exception("One or more env vars is/are null/empty");
        }
    }
}