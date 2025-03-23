namespace SharedModels.Utils
{
    public static class PolicyUtils
    {
        public static bool EvaluateCondition(object variableValue, string op, double value)
        {
            switch (op)
            {
                case ">": return Convert.ToDouble(variableValue) > value;
                case "<": return Convert.ToDouble(variableValue) < value;
                case "=": return Convert.ToDouble(variableValue) == value;
                default: throw new InvalidOperationException("Operador inválido");
            }
        }
    }
}