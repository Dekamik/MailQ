FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src
COPY ["MailQ.Consumer/MailQ.Consumer.csproj", "MailQ.Consumer/"]
RUN dotnet restore "MailQ.Consumer/MailQ.Consumer.csproj"
COPY . .
WORKDIR "/src/MailQ.Consumer"
RUN dotnet build "MailQ.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MailQ.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MailQ.Consumer.dll"]
