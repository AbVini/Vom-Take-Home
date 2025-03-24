#!/bin/bash

# Setup Script for Entity Framework Core Database

echo "Setting up the database for the backend..."
cd "$(dirname "$0")/.."

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null
then
 echo "Error: .NET SDK is not installed. Please install it from https://dotnet.microsoft.com/download."
 exit 1
fi

# Check if dotnet-ef is installed
if ! dotnet tool list -g | grep dotnet-ef &> /dev/null
then
 echo "dotnet-ef is not installed. Installing it globally..."
 dotnet tool install --global dotnet-ef
else
 echo "dotnet-ef is already installed."
fi

# Create the initial migration
echo "Creating the initial migration..."
dotnet ef migrations add InitialCreate --project ./ConfigBackend/

# Apply the migration to the database
echo "Applying the migration to the database..."
dotnet ef database update --project ./ConfigBackend/

echo "Database setup completed successfully!"