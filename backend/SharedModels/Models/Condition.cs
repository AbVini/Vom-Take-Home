using System.Text.Json.Serialization;

namespace SharedModels.Models
{
    public class Condition
    {
        public int Id { get; set; }
        public string InputVariable { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public double Value { get; set; }
        public double? DecisionValue { get; set; }

        public int? TrueConditionId { get; set; }
        public Condition? TrueCondition { get; set; }

        public int? FalseConditionId { get; set; }
        public Condition? FalseCondition { get; set; }

        public int PolicyId { get; set; }

        [JsonIgnore]
        public Policy? Policy { get; set; }
    }
}