FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["CustomerManagement.Api.Web/CustomerManagement.Api.Web.csproj", "CustomerManagement.Api.Web/"]
COPY ["CustomerManagement.Base/CustomerManagement.Base.csproj", "CustomerManagement.Base/"]
RUN dotnet restore "CustomerManagement.Api.Web/CustomerManagement.Api.Web.csproj"
COPY . .
WORKDIR "/src/CustomerManagement.Api.Web"
RUN dotnet build "CustomerManagement.Api.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CustomerManagement.Api.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CustomerManagement.Api.Web.dll"]
