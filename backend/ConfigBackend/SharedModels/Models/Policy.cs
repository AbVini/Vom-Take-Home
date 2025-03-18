using System.Text.Json.Serialization;

namespace SharedModels.Models;

public class Policy
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Condition? StartCondition { get; set; } 
    public List<Condition> Conditions { get; set; } = new(); 
}