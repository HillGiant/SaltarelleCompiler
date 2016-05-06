namespace TypeScriptModel.TypeSystem.Parameters
{
    using TypeScriptModel.TypeSystem.Types;

    public class TsParameter
    {
        public string Name { get; private set; }

        public TsType Type { get; private set; }

        public bool Optional { get; private set; }

        public bool ParamArray { get; private set; }

        public AccessibilityModifier? Modifier {get; private set; }

        public TsParameter(string name, TsType type, bool optional, bool paramArray, AccessibilityModifier? modifier)
        {
            Name = name;
            Type = type;
            Optional = optional;
            ParamArray = paramArray;
            Modifier = modifier;
        }
    }
}