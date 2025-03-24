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

        app.MapPost("/api/policies", async (PolicyDTO policyDto, AppDbContext dbContext) =>
        {
            var policy = new Policy
            {
                Name = policyDto.Name,
                Conditions = new List<Condition>()
            };

            var conditions = policyDto.Conditions.Select(dto => new Condition
            {
                InputVariable = dto.InputVariable,
                Operator = dto.Operator,
                Value = dto.Value,
                DecisionValue = dto.DecisionValue,
                Policy = policy
            }).ToList();

            for (int i = 0; i < policyDto.Conditions.Count; i++)
            {
                var conditionDto = policyDto.Conditions[i];
                var currentCondition = conditions[i];

                if (conditionDto.TrueConditionIndex.HasValue)
                {
                    currentCondition.TrueCondition = conditions[conditionDto.TrueConditionIndex.Value];
                }

                if (conditionDto.FalseConditionIndex.HasValue)
                {
                    currentCondition.FalseCondition = conditions[conditionDto.FalseConditionIndex.Value];
                }
            }

            policy.StartCondition = conditions.FirstOrDefault();

            await dbContext.Policies.AddAsync(policy);
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
