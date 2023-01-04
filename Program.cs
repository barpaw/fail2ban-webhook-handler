using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () =>
{
    return Results.Ok("Working");
});

app.MapGet("/health", () =>
{
    return Results.Ok();
});

app.MapPost("/handle-fail2ban-webhook", async (Fail2BanWebhook fail2BanWebhook, IHttpClientFactory httpClientFactory) =>
{

    var client = httpClientFactory.CreateClient();
    
    string matrix_notifier_url = Environment.GetEnvironmentVariable("MATRIX_NOTIFIER_URL");
    string matrix_notifier_message_header = Environment.GetEnvironmentVariable("MATRIX_NOTIFIER_MESSAGE_HEADER");

    string matrix_homeserver_url = Environment.GetEnvironmentVariable("MATRIX_HOMESERVER_URL");
    string matrix_homeserver_user = Environment.GetEnvironmentVariable("MATRIX_HOMESERVER_USER");
    string matrix_homeserver_passwd = Environment.GetEnvironmentVariable("MATRIX_HOMESERVER_PASSWD");
    string matrix_homeserver_room = Environment.GetEnvironmentVariable("MATRIX_HOMESERVER_ROOM");

    StringBuilder sb = new StringBuilder();
    
    sb.Append(matrix_notifier_message_header);
    
    sb.Append(Environment.NewLine);
    sb.Append("- - -");
    sb.Append(fail2BanWebhook.Message);
    sb.Append(Environment.NewLine);

    string message = sb.ToString();

    app.Logger.LogInformation(message);

    var matrixNotifierBody = new MatrixNotifier(matrix_homeserver_url, matrix_homeserver_user, matrix_homeserver_passwd, matrix_homeserver_room, message);

    string jsonString = JsonSerializer.Serialize(matrixNotifierBody);

    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

    await client.PostAsync(matrix_notifier_url, content);

    return Results.Ok("Handled");
});

app.Run("http://localhost:3411");


public record Fail2BanWebhook(
    [property: JsonPropertyName("message")] string Message

);

public record MatrixNotifier(
    [property: JsonPropertyName("matrixHomeserverURL")] string MatrixHomeserverURL,
    [property: JsonPropertyName("matrixHomeserverUser")] string MatrixHomeserverUser,
    [property: JsonPropertyName("matrixHomeserverPasswd")] string MatrixHomeserverPasswd,
    [property: JsonPropertyName("matrixHomeserverRoom")] string MatrixHomeserverRoom,
    [property: JsonPropertyName("message")] string Message
);