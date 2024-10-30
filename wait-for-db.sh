#!/bin/bash
# wait-for-db.sh

set -e

host="$1"
shift
cmd="$@"

until mysqladmin ping -h "$host" --silent; do
  >&2 echo "Waiting for MySQL to be ready..."
  sleep 2
done

>&2 echo "MySQL is up - executing command"
exec $cmd
