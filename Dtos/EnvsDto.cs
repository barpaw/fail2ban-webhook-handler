namespace Fail2BanWebhookHandler.Dtos;

public class EnvsDto
{
    public string? MatrixUserCheckApi { get; set; }
    public string? MatrixUserCheckApiToken { get; set; }
    public string? MatrixNotifierUrl { get; set; }
    public string? MatrixNotifierMessageHeader { get; set; }
    public string? MatrixHomeserverUrl { get; set; }
    public string? MatrixHomeserverUser { get; set; }
    public string? MatrixHomeserverPasswd { get; set; }
    public string? MatrixHomeserverRoom { get; set; }
    public string? MatrixMessageFooter { get; set; }
}