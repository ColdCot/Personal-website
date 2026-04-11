# STAGE 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (Optimizes layer caching)
COPY ["Personal_website/Personal_website.csproj", "Personal_website/"]
RUN dotnet restore "Personal_website/Personal_website.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "Personal_website/Personal_website.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID

# Standard ports for ASP.NET
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Personal_website.dll"]
