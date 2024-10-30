# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only the solution and project files for faster caching
COPY ./IlluviumTest.sln ./
COPY ./IlluviumTest/IlluviumTest.csproj ./IlluviumTest/
COPY ../IlluviumTest.Test/IlluviumTest.Test.csproj ./IlluviumTest.Test/

# Restore dependencies for the solution
RUN dotnet restore

# Copy the remaining files and build the project
COPY . .
WORKDIR /app/IlluviumTest
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "YourApp.dll"]  # Default entrypoint, overridden for migrations
