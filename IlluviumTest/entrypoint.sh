#!/bin/bash
set -e

# Wait for MySQL to be ready
until mysqladmin ping -h"db" -P 3306 --silent; do
  echo "Waiting for MySQL to be ready..."
  sleep 2
done

# Run migrations
dotnet ef database update

# Start MySQL server
exec mysqld
