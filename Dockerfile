FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 3411

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Fail2BanWebhookHandler.csproj", "./"]
RUN dotnet restore "Fail2BanWebhookHandler.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Fail2BanWebhookHandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fail2BanWebhookHandler.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER 65534
EXPOSE 3411
ENV ASPNETCORE_URLS=http://*:3411
ENTRYPOINT ["dotnet", "Fail2BanWebhookHandler.dll"]
