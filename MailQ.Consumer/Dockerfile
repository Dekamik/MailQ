FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MailQ.Worker/MailQ.Worker.csproj", "MailQ.Worker/"]
RUN dotnet restore "MailQ.Worker/MailQ.Worker.csproj"
COPY . .
WORKDIR "/src/MailQ.Worker"
RUN dotnet build "MailQ.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MailQ.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MailQ.Worker.dll"]
