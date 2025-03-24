#!/bin/bash

# Startup Script for Backend Services

# Define colors for terminal output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}Starting ConfigBackend and ExecutionEngine...${NC}"

cd "$(dirname "$0")/.."

# Start ConfigBackend
echo -e "${YELLOW}Starting ConfigBackend on https://localhost:7176...${NC}"
dotnet run --launch-profile https --project ./ConfigBackend/ &

# Start ExecutionEngine
echo -e "${YELLOW}Starting ExecutionEngine on https://localhost:7276...${NC}"
dotnet run --launch-profile https --project ./ExecutionEngine/ &

# Highlight Swagger URLs
echo -e "${RED}*** Both services are now running! ***${NC}"
echo -e "${GREEN}ConfigBackend Swagger URL:${NC} ${YELLOW}https://localhost:7176/swagger/index.html${NC}"
echo -e "${GREEN}ExecutionEngine Swagger URL:${NC} ${YELLOW}https://localhost:7276/swagger/index.html${NC}"

# Optional: Keep the terminal open to monitor logs
echo -e "${RED}Press Ctrl+C to stop both services.${NC}"
wait