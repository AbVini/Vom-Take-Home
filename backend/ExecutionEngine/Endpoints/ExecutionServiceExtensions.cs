using ExecutionEngine.Data;
using Microsoft.EntityFrameworkCore;
using SharedModels.Models;

namespace ExecutionEngine.Endpoints;

public static class ExecutionServiceExtensions
{
    public static void MapExecutionRoutes(this WebApplication app)
    {
        app.MapPost("/api/execution/execute", async (Dictionary<string, object> inputData, AppDbContext db) =>
        {
            try
            {
                var policy = await db.Policies.Include(p => p.Conditions).FirstOrDefaultAsync();
                if (policy == null)
                {
                    return Results.NotFound(new { Message = "No policy found." });
                }

                if (policy.StartCondition == null)
                {
                    return Results.Problem("No start condition defined for the policy.");
                }

                var resultCondition = NavigateTree(policy, inputData);

                if (resultCondition?.DecisionValue.HasValue == true)
                {
                    return Results.Ok(new { decision = resultCondition.DecisionValue.Value });
                }

                return Results.Problem("Policy evaluation failed: No valid decision was reached.");
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem($"An unexpected error occurred: {ex.Message}");
            }
        });
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
            Console.WriteLine($"Evaluating condition ID: {currentCondition.Id}");

            if (currentCondition.DecisionValue.HasValue)
            {
                Console.WriteLine($"Reached terminal node with DecisionValue: {currentCondition.DecisionValue}");
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

                if (EvaluateCondition(numericValue, currentCondition.Operator, currentCondition.Value))
                {
                    Console.WriteLine($"Condition met. Moving to TrueCondition ID: {currentCondition.TrueConditionId}");
                    currentCondition = currentCondition.TrueCondition;
                }
                else
                {
                    Console.WriteLine($"Condition not met. Moving to FalseCondition ID: {currentCondition.FalseConditionId}");
                    currentCondition = currentCondition.FalseCondition;
                }
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