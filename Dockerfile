# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only the solution and project files for faster caching
COPY ./IlluviumTest.sln ./
COPY ./IlluviumTest/IlluviumTest.csproj ./IlluviumTest/
COPY ./IlluviumTest.Test/IlluviumTest.Test.csproj ./IlluviumTest.Test/
COPY ./IlluviumTest/entrypoint.sh ./IlluviumTest/

# Restore dependencies for the solution
RUN dotnet restore

# Install the dotnet-ef tool globally
RUN dotnet tool install --global dotnet-ef

# Ensure the PATH environment variable includes the .NET tools
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy the remaining files and build the project
COPY . .
WORKDIR /app/IlluviumTest
RUN dotnet publish -c Release -o /app/publish
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh


