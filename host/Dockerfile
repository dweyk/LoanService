FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["back-end-test/back-end-test.csproj", "back-end-test/"]
RUN dotnet restore "back-end-test/back-end-test.csproj"
COPY . .
WORKDIR "/src/back-end-test"
RUN dotnet build "back-end-test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "back-end-test.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "back-end-test.dll"]
