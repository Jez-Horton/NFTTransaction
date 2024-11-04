# Enable error handling
$ErrorActionPreference = "Stop"

Write-Host "Starting Docker Compose services..."
docker-compose up -d --build

Write-Host "Waiting for database to be healthy..."
# Replace 'illuvium-db-1' with the actual container name
$dbContainerName = "illuvium-db-1"

# Check the health status in a loop
while ((docker inspect -f '{{.State.Health.Status}}' $dbContainerName) -ne "healthy") {
    Write-Host "Waiting for the database to be ready..."
    Start-Sleep -Seconds 5
}

Write-Host "Database is healthy. Running migrations..."
# Navigate to the project directory
Set-Location -Path "./IlluviumTest"

# Run Entity Framework migrations
dotnet ef database update

Write-Host "All done!"
