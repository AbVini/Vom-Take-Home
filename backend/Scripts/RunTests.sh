#!/bin/bash

# Script to Run Unit Tests

echo "Running unit tests for the backend..."

cd "$(dirname "$0")/.."
# Navigate to the tests directory
cd ./ExecutionEngine.Tests


# Run the tests
dotnet test

# Check the result of the tests
if [ $? -eq 0 ]; then
 echo "All tests passed successfully!"
else
 echo "Some tests failed. Please check the output above for details."
 exit 1
fi