#!/bin/bash
set -e

/opt/mssql/bin/sqlservr &
SQL_PID=$!

echo "Waiting for SQL Server to start..."
for i in $(seq 1 60); do
  if /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" >/dev/null 2>&1; then
    echo "SQL Server is up."
    break
  fi
  if [ "$i" -eq 60 ]; then
    echo "SQL Server did not start in time." >&2
    exit 1
  fi
  sleep 1
done

echo "Applying initialization scripts..."
for f in /docker-entrypoint-initdb.d/*.sql; do
  if [ -f "$f" ]; then
    echo "Running $f"
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i "$f"
  fi
done

echo "Database initialization complete."
wait "$SQL_PID"