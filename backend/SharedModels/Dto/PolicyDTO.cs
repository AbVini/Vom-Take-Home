
namespace SharedModels.Dto;

public class PolicyDTO
{
    public string Name { get; set; } = string.Empty;
    public List<ConditionCreateDto> Conditions { get; set; } = new();
}

public class PolicyDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartConditionId { get; set; }
    public List<ConditionDto> Conditions { get; set; } = new();
}


public class PolicyOutputDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ConditionDto
{
    public int Id { get; set; } // Agora o ID será gerenciado pelo EF Core, mas ainda estará presente no DTO
    public string InputVariable { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public double Value { get; set; }
    public double? DecisionValue { get; set; }
    public int? TrueConditionId { get; set; }
    public int? FalseConditionId { get; set; }
}

public class ConditionCreateDto
{
    public string InputVariable { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public double Value { get; set; }
    public double? DecisionValue { get; set; }
    public int? TrueConditionIndex { get; set; } // Índice ao invés de ID
    public int? FalseConditionIndex { get; set; } // Índice ao invés de ID
}