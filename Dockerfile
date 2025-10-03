# Stage 1: Build the application using .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files, then restore dependencies (for layer caching)
COPY *.sln .
COPY Common.Address/*.csproj ./Common.Address/
COPY Common.Address.Test/*.csproj ./Common.Address.Test/
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish the main application to a clean folder
RUN dotnet publish Common.Address/Common.Address.csproj -c Release -o /app/publish

# Stage 2: Create runtime image using ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Inject environment variable securely
# ENV ADDRESS_API_KEY=${ADDRESS_API_KEY}

# Expose default HTTP port
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "Common.Address.dll"]