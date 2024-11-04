#!/bin/bash
set -e  # Exit immediately if a command exits with a non-zero status

echo "Starting Docker Compose services..."
docker-compose up -d --build

echo "Waiting for database to be healthy..."
# Replace 'illuvium-db-1' with the actual container name
DB_CONTAINER_NAME="illuvium-db-1"

# Loop until the database container's health status is "healthy"
until [ "$(docker inspect -f '{{.State.Health.Status}}' "$DB_CONTAINER_NAME")" == "healthy" ]; do
  echo "Waiting for the database to be ready..."
  sleep 5
done

echo "Database is healthy. Running migrations..."
cd ./IlluviumTest/
dotnet ef database update

echo "All done!"
