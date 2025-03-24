#!/bin/bash

# Script to Clean the Backend Environment
cd "$(dirname "$0")/.."

echo "Cleaning up the backend environment..."

# Stop any running services on ports 7176 and 7276
echo "Stopping services running on ports 7176 and 7276..."
fuser -k 7176/tcp &> /dev/null
fuser -k 7276/tcp &> /dev/null

# Remove the SQLite database file
if [ -f "./ConfigBackend/policies.db" ]; then
 echo "Removing the SQLite database file (policies.db)..."
 rm ./ConfigBackend/policies.db
else
 echo "No SQLite database file found to remove."
fi

# Remove migrations folder
if [ -d "./ConfigBackend/Migrations" ]; then
 echo "Removing the Migrations folder..."
 rm -rf ./ConfigBackend/Migrations
else
 echo "No Migrations folder found to remove."
fi

echo "Environment cleaned successfully!"