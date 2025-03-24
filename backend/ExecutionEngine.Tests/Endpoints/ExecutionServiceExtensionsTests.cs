using Xunit;
using Microsoft.EntityFrameworkCore;
using ExecutionEngine.Data;
using SharedModels.Models;
using System.Collections.Generic;

namespace ExecutionEngine.Tests.Endpoints
{
 public class ExecutionServiceExtensionsTests
 {
     private AppDbContext GetInMemoryDbContext()
     {
         var options = new DbContextOptionsBuilder<AppDbContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;

         return new AppDbContext(options);
     }

     [Fact]
     public async Task ExecutePolicy_ReturnsDecision_WhenPolicyIsValid()
     {
         // Arrange
         var dbContext = GetInMemoryDbContext();

         // Seed a policy and conditions
         var policy = new Policy
         {
             Name = "Test Policy",
             StartCondition = new Condition
             {
                 InputVariable = "age",
                 Operator = ">",
                 Value = 18,
                 TrueCondition = new Condition
                 {
                     DecisionValue = 1000
                 },
                 FalseCondition = new Condition
                 {
                     DecisionValue = 0
                 }
             }
         };

         dbContext.Policies.Add(policy);
         await dbContext.SaveChangesAsync();

         var inputData = new Dictionary<string, object>
         {
             { "age", 25 }
         };

         // Act
         var resultCondition = NavigateTree(policy, inputData);

         // Assert
         Assert.NotNull(resultCondition);
         Assert.Equal(1000, resultCondition.DecisionValue);
     }

     [Fact]
     public async Task ExecutePolicy_ReturnsZeroDecision_WhenConditionFails()
     {
         // Arrange
         var dbContext = GetInMemoryDbContext();

         // Seed a policy and conditions
         var policy = new Policy
         {
             Name = "Test Policy",
             StartCondition = new Condition
             {
                 InputVariable = "age",
                 Operator = ">",
                 Value = 18,
                 TrueCondition = new Condition
                 {
                     DecisionValue = 1000
                 },
                 FalseCondition = new Condition
                 {
                     DecisionValue = 0
                 }
             }
         };

         dbContext.Policies.Add(policy);
         await dbContext.SaveChangesAsync();

         var inputData = new Dictionary<string, object>
         {
             { "age", 15 }
         };

         // Act
         var resultCondition = NavigateTree(policy, inputData);

         // Assert
         Assert.NotNull(resultCondition);
         Assert.Equal(0, resultCondition.DecisionValue);
     }

     [Fact]
     public void ExecutePolicy_ThrowsArgumentException_WhenInputVariableIsMissing()
     {
         // Arrange
         var policy = new Policy
         {
             Name = "Test Policy",
             StartCondition = new Condition
             {
                 InputVariable = "age",
                 Operator = ">",
                 Value = 18,
                 TrueCondition = new Condition
                 {
                     DecisionValue = 1000
                 },
                 FalseCondition = new Condition
                 {
                     DecisionValue = 0
                 }
             }
         };

         var inputData = new Dictionary<string, object>(); // Missing "age"

         // Act & Assert
         var exception = Assert.Throws<ArgumentException>(() =>
             NavigateTree(policy, inputData)
         );

         Assert.Equal("Variable 'age' is missing in the input data.", exception.Message);
     }

     [Fact]
     public void ExecutePolicy_ThrowsInvalidOperationException_WhenStartConditionIsNull()
     {
         // Arrange
         var policy = new Policy
         {
             Name = "Test Policy",
             StartCondition = null // No start condition
         };

         var inputData = new Dictionary<string, object>
         {
             { "age", 25 }
         };

         // Act & Assert
         var exception = Assert.Throws<InvalidOperationException>(() =>
             NavigateTree(policy, inputData)
         );

         Assert.Equal("The policy does not have a valid start condition.", exception.Message);
     }

     private static Condition? NavigateTree(Policy policy, Dictionary<string, object> inputData)
     {
         if (policy.StartCondition == null)
         {
             throw new InvalidOperationException("The policy does not have a valid start condition.");
         }

         var currentCondition = policy.StartCondition;

         while (currentCondition != null)
         {
             if (currentCondition.DecisionValue.HasValue)
             {
                 return currentCondition;
             }

             if (!string.IsNullOrEmpty(currentCondition.InputVariable))
             {
                 if (!inputData.TryGetValue(currentCondition.InputVariable, out var variableValue))
                 {
                     throw new ArgumentException($"Variable '{currentCondition.InputVariable}' is missing in the input data.");
                 }

                 if (!double.TryParse(variableValue.ToString(), out var numericValue))
                 {
                     throw new ArgumentException($"Variable '{currentCondition.InputVariable}' must be a numeric value.");
                 }

                 currentCondition = EvaluateCondition(numericValue, currentCondition.Operator, currentCondition.Value)
                     ? currentCondition.TrueCondition
                     : currentCondition.FalseCondition;
             }
             else
             {
                 throw new InvalidOperationException($"Condition ID {currentCondition.Id} does not have a valid InputVariable or DecisionValue.");
             }
         }

         throw new InvalidOperationException("Policy evaluation failed: No valid decision was reached.");
     }

     private static bool EvaluateCondition(double variableValue, string operatorSymbol, double conditionValue)
     {
         return operatorSymbol switch
         {
             "=" => variableValue == conditionValue,
             "<" => variableValue < conditionValue,
             "<=" => variableValue <= conditionValue,
             ">" => variableValue > conditionValue,
             ">=" => variableValue >= conditionValue,
             _ => throw new ArgumentException($"Unsupported operator: {operatorSymbol}")
         };
     }
 }
}