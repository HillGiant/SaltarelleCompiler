namespace TypeScriptModel.TypeSystem
{
    public class TsParameter
    {
        public string Name { get; private set; }

        public TsType Type { get; private set; }

        public bool Optional { get; private set; }

        public bool ParamArray { get; private set; }

        public TsParameter(string name, TsType type, bool optional, bool paramArray)
        {
            Name = name;
            Type = type;
            Optional = optional;
            ParamArray = paramArray;
        }
    }
}