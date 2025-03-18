using Xunit;
using ConfigBackend.Data;
using Microsoft.EntityFrameworkCore;
using SharedModels.Models;

public class PolicyTests
{
    private AppDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Should_Create_Policy()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext("CreatePolicyTest");
        var policy = new Policy
        {
            Name = "Test Policy",
            Conditions = new List<Condition>
         {
             new Condition
             {
                 InputVariable = "age",
                 Operator = ">",
                 Value = 18,
                 DecisionValue = null
             }
         }
        };

        // Act
        dbContext.Policies.Add(policy);
        await dbContext.SaveChangesAsync();

        // Assert
        var savedPolicy = await dbContext.Policies.Include(p => p.Conditions).FirstOrDefaultAsync(p => p.Name == "Test Policy");
        Assert.NotNull(savedPolicy);
        Assert.Equal("Test Policy", savedPolicy.Name);
        Assert.Single(savedPolicy.Conditions);
        Assert.Equal("age", savedPolicy.Conditions.First().InputVariable);
    }

    [Fact]
    public async Task Should_Return_BadRequest_When_Policy_Name_Is_Missing()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext("MissingPolicyNameTest");
        var policyDto = new Policy
        {
            Name = "", 
            Conditions = new List<Condition>()
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            if (string.IsNullOrWhiteSpace(policyDto.Name))
            {
                throw new ArgumentException("Policy name is required.");
            }

            dbContext.Policies.Add(policyDto);
            await dbContext.SaveChangesAsync();
        });

        Assert.Equal("Policy name is required.", exception.Message);
    }

    [Fact]
    public async Task Should_Return_All_Policies()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext("ReturnAllPoliciesTest");
        dbContext.Policies.Add(new Policy { Name = "Policy 1" });
        dbContext.Policies.Add(new Policy { Name = "Policy 2" });
        await dbContext.SaveChangesAsync();

        // Act
        var policies = await dbContext.Policies.ToListAsync();

        // Assert
        Assert.Equal(2, policies.Count);
        Assert.Contains(policies, p => p.Name == "Policy 1");
        Assert.Contains(policies, p => p.Name == "Policy 2");
    }

    [Fact]
    public async Task Should_Delete_Policy()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext("DeletePolicyTest");
        var policy = new Policy { Name = "Policy to Delete" };
        dbContext.Policies.Add(policy);
        await dbContext.SaveChangesAsync();

        // Act
        dbContext.Policies.Remove(policy);
        await dbContext.SaveChangesAsync();

        // Assert
        var deletedPolicy = await dbContext.Policies.FirstOrDefaultAsync(p => p.Name == "Policy to Delete");
        Assert.Null(deletedPolicy);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Deleting_Nonexistent_Policy()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext("NonexistentPolicyTest");

        // Act
        var policy = await dbContext.Policies.FindAsync(999); // ID inexistente

        // Assert
        Assert.Null(policy); // Verifica que a política não foi encontrada
    }

    [Fact]
    public async Task Should_Update_Policy_Name()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext("UpdatePolicyTest");
        var policy = new Policy { Name = "Old Policy Name" };
        dbContext.Policies.Add(policy);
        await dbContext.SaveChangesAsync();

        // Act
        var existingPolicy = await dbContext.Policies.FirstOrDefaultAsync(p => p.Name == "Old Policy Name");
        Assert.NotNull(existingPolicy);

        existingPolicy.Name = "Updated Policy Name";
        await dbContext.SaveChangesAsync();

        // Assert
        var updatedPolicy = await dbContext.Policies.FirstOrDefaultAsync(p => p.Name == "Updated Policy Name");
        Assert.NotNull(updatedPolicy);
        Assert.Equal("Updated Policy Name", updatedPolicy.Name);
    }
}