# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["EpicShowdown.API/EpicShowdown.API.csproj", "EpicShowdown.API/"]
COPY ["EpicShowdown-Backend.sln", "./"]

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the application
RUN dotnet publish "EpicShowdown.API/EpicShowdown.API.csproj" -c Release -o /app/publish

# Use the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port 80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "EpicShowdown.API.dll"] 