FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src
COPY ["MailQ.IntegrationTest/MailQ.IntegrationTest.csproj", "MailQ.IntegrationTest/"]
RUN dotnet restore "MailQ.IntegrationTest/MailQ.IntegrationTest.csproj"
COPY . .
WORKDIR "/src/MailQ.IntegrationTest"
RUN dotnet build "MailQ.IntegrationTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MailQ.IntegrationTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MailQ.IntegrationTest.dll"]
