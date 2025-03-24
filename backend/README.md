# Backend Implementation for Vom Take-home Challenge

Welcome to the backend implementation of the Vom Take-home Challenge! This document provides all the necessary information to understand, configure, and run the backend application.

## Overview

The backend is responsible for:
1. Managing decision policies via REST endpoints (`ConfigBackend`).
2. Executing decision policies based on input data (`ExecutionEngine`).

The backend interacts with a SQLite database (`PolicyDB`) to store and retrieve policies.

To simplify the setup and execution process, auxiliary scripts are provided in the `Scripts` folder.

---

## Technologies Used

Here are the technologies chosen for the development of the backend, along with a brief justification for each choice:

- **C# + ASP.NET Core**  
C# and ASP.NET Core were selected for their robustness, performance, and widespread adoption in building REST APIs. ASP.NET Core offers native support for modern development practices such as dependency injection, modularity, and high performance, making it ideal for scalable backend applications.

- **Entity Framework Core (EF Core)**  
EF Core was used as the ORM (Object-Relational Mapping) tool to simplify interactions between the code and the database. It allows queries to be written declaratively using LINQ, reducing the need for manual SQL and simplifying complex operations.

- **SQLite**  
SQLite was chosen as the database for its lightweight nature and ease of configuration. It is ideal for small projects or prototypes, enabling fast execution and efficient integration with EF Core. Additionally, its portability ensures that the environment is easy to set up and run.

- **Unit Testing: xUnit + EF Core InMemory**  
Unit tests were implemented using xUnit for test organization and EF Core's InMemory provider to simulate the database during testing. This ensures robust validation of backend functionality without requiring a physical database.

---

## How to Use the Scripts

All auxiliary scripts are located in the `Scripts` folder inside the `backend` directory. Below is a description of each script:

### 1. SetupDatabase.sh
This script automates the configuration of the database, including creating and applying migrations.
```bash
./Scripts/SetupDatabase.sh
```
### 2. RunTests.sh
This script runs all unit tests for the backend.
```bash
./Scripts/RunTests.sh
```
### 3. Startup.sh
This script starts both ConfigBackend and ExecutionEngine services simultaneously. After running the script, you will see the Swagger URLs highlighted in the terminal for easy access.
```bash
./Scripts/Startup.sh
```
## Example Payloads
Here are examples of payloads for testing the backend:
### Creating a Policy (POST /api/policies)
```JSON
{
"name": "Simple Policy",
"conditions": [
  {
    "inputVariable": "age",
    "operator": ">",
    "value": 18,
    "decisionValue": null,
    "trueConditionIndex": 1,
    "falseConditionIndex": 2
  },
  {
    "inputVariable": "income",
    "operator": ">",
    "value": 1000,
    "decisionValue": 1000,
    "trueConditionIndex": null,
    "falseConditionIndex": null
  },
  {
    "inputVariable": "",
    "operator": "",
    "value": 0,
    "decisionValue": 0,
    "trueConditionIndex": null,
    "falseConditionIndex": null
  }
]
}
```
### Executing a Policy (POST /api/execution/execute)
```Json
{
"age": 25,
"income": 3000
}
```
### Expected Response:
```Json
{
"decision": 1000.0
}
```
# Running Tests
To validate the backend functionality, unit tests have been implemented. Follow these steps to run the tests:

## Step 1: Navigate to the Tests Directory
Go to the tests directory:
```bash
cd ../ExecutionEngine.Test
```
## Step 2: Run the Tests
Execute the tests using the following command:
```bash
dotnet test
```
The results will indicate whether all tests passed successfully.

# Design Decisions
## Why SQLite?
SQLite was chosen for its simplicity and portability, making it ideal for this challenge. It eliminates the need for complex database setups, allowing the focus to remain on backend logic.

## Why Entity Framework Core?
EF Core simplifies database interactions, allowing us to focus on business logic rather than writing SQL manually. Its integration with LINQ makes querying intuitive and reduces boilerplate code.

## Why xUnit?
xUnit is widely adopted for .NET testing and provides a clean, organized way to write unit tests. Combined with EF Core's InMemory provider, it enables robust testing without requiring a physical database.


--- 
Thank you for reviewing this backend implementation! If you have any questions or feedback, feel free to reach out. I hope this solution meets the expectations of the challenge and demonstrates our ability to deliver a functional and well-tested backend.