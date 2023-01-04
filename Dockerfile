FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Fail2BanWebhookHandler.csproj", "./"]
RUN dotnet restore "Fail2BanWebhookHandler.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Fail2BanWebhookHandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fail2BanWebhookHandler.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fail2BanWebhookHandler.dll"]
