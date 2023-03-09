using System.Text.Json.Serialization;
using Serilog;
using Fail2BanWebhookHandler.Commands;
using Fail2BanWebhookHandler.Services;
using MediatR;
using Polly;
using Serilog.Sinks.SystemConsole.Themes;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Host.UseSerilog();
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

    builder.Services.AddTransient<IEnvironmentService, EnvironmentService>();
    builder.Services.AddTransient<IMessageService, MessageService>();
    builder.Services.AddTransient<IApiService, ApiService>();

    builder.Services.AddHttpClient("api")
        .AddTransientHttpErrorPolicy(policyBuilder =>
            policyBuilder.WaitAndRetryAsync(
                10, retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber))));

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapGet("/", () => { return Results.Ok("Working"); });

    app.MapGet("/health", () => { return Results.Ok(); });

    app.MapPost("/handle-fail2ban-webhook", async (IMediator mediator, Fail2BanWebhook fail2BanWebhook) =>
    {
        var response = await mediator.Send(new HandleFail2BanWebhookCommand(fail2BanWebhook));

        return Results.Ok(response);
    });

    app.Run("http://0.0.0.0:3411");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public record Fail2BanWebhook(
    [property: JsonPropertyName("message")]
    string Message
);

public record MatrixUserCheck(
    [property: JsonPropertyName("ip")] string Ip,
    [property: JsonPropertyName("token")] string Token
);

public record MatrixNotifier(
    [property: JsonPropertyName("matrixHomeserverURL")]
    string MatrixHomeserverURL,
    [property: JsonPropertyName("matrixHomeserverUser")]
    string MatrixHomeserverUser,
    [property: JsonPropertyName("matrixHomeserverPasswd")]
    string MatrixHomeserverPasswd,
    [property: JsonPropertyName("matrixHomeserverRoom")]
    string MatrixHomeserverRoom,
    [property: JsonPropertyName("message")]
    string Message
);

public record MatrixUserCheckResult(
    [property: JsonPropertyName("user_id")]
    string UserId,
    [property: JsonPropertyName("ip")] string Ip,
    [property: JsonPropertyName("user_agent")]
    string UserAgent,
    [property: JsonPropertyName("last_seen")]
    string LastSeen
);