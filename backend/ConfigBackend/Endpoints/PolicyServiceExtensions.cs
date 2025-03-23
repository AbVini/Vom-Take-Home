using ConfigBackend.Data;
using Microsoft.EntityFrameworkCore;
using SharedModels.Dto;
using SharedModels.Models;
using System.Linq;

namespace ConfigBackend.Endpoints;

public static class PolicyServiceExtensions
{
    public static void MapPolicyEndpoints(this WebApplication app)
    {
        app.MapGet("/api/policies", async (AppDbContext db) =>
        {
            var policies = await db.Policies.ToListAsync();

            var policyDtos = policies.Select(p => new PolicyOutputDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            return Results.Ok(policyDtos);
        });
        app.MapGet("/api/policies/{id}", async (int id, AppDbContext db) =>
        {
            try
            {
                var policy = await db.Policies.Include(p => p.Conditions).FirstOrDefaultAsync(p => p.Id == id);
                if (policy == null)
                {
                    return Results.NotFound(new { Message = $"Policy with ID {id} not found." });
                }

                var policyDto = new PolicyDetailDto
                {
                    Id = policy.Id,
                    Name = policy.Name,
                    StartConditionId = policy.Conditions.FirstOrDefault(c => c == policy.StartCondition).Id,
                    Conditions = policy.Conditions.Select(c => new ConditionDto
                    {
                        Id = c.Id,
                        InputVariable = c.InputVariable,
                        Operator = c.Operator,
                        Value = c.Value,
                        DecisionValue = c.DecisionValue,
                        TrueConditionId = c.TrueCondition?.Id,
                        FalseConditionId = c.FalseCondition?.Id
                    }).ToList()
                };

                return Results.Ok(policyDto);
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while fetching the policy: {ex.Message}");
            }
        });

        app.MapPost("/api/policies", async (PolicyDetailDto policyDto, AppDbContext dbContext) =>
        {
            var policy = new Policy
            {
                Name = policyDto.Name,
                Conditions = new List<Condition>()
            };

            var conditionMap = new Dictionary<int, Condition>();
            foreach (ConditionDto conditionDto in policyDto.Conditions)
            {
                var condition = new Condition
                {
                    Id = conditionDto.Id, 
                    InputVariable = conditionDto.InputVariable,
                    Operator = conditionDto.Operator,
                    Value = conditionDto.Value,
                    DecisionValue = conditionDto.DecisionValue,
                    Policy = policy
                };
                conditionMap[conditionDto.Id] = condition;
                policy.Conditions.Add(condition);
            }

            foreach (var conditionDto in policyDto.Conditions)
            {
                if (conditionDto.TrueConditionId.HasValue)
                {
                    conditionMap[conditionDto.Id].TrueCondition = conditionMap[conditionDto.TrueConditionId.Value];
                }
                if (conditionDto.FalseConditionId.HasValue)
                {
                    conditionMap[conditionDto.Id].FalseCondition = conditionMap[conditionDto.FalseConditionId.Value];
                }
            }

            policy.StartCondition = conditionMap[policyDto.StartConditionId];

            dbContext.Policies.Add(policy);
            await dbContext.SaveChangesAsync();

            return Results.Ok(policy);
        });
        
        app.MapPut("/api/policies/{id}", async (int id, Policy updatedPolicy, AppDbContext db) =>
        {
            var policy = await db.Policies.Include(p => p.Conditions).FirstOrDefaultAsync(p => p.Id == id);
            if (policy is null) return Results.NotFound();

            policy.Name = updatedPolicy.Name;
            policy.Conditions = updatedPolicy.Conditions;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.MapDelete("/api/policies/{id}", async (int id, AppDbContext db) =>
        {
            try
            {
                var policy = await db.Policies.FindAsync(id);
                if (policy == null)
                {
                    return Results.NotFound(new { Message = $"Policy with ID {id} not found." });
                }

                db.Policies.Remove(policy);
                await db.SaveChangesAsync();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while deleting the policy: {ex.Message}");
            }
        });
    }

}
