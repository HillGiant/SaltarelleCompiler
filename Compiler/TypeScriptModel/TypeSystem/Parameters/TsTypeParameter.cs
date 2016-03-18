namespace TypeScriptModel.TypeSystem
{
    public class TsTypeParameter
    {
        public string Name { get; private set; }

        public TsType Constraint { get; private set; }

        public TsTypeParameter(string name, TsType constraint)
        {
            Name = name;
            Constraint = constraint;
        }
    }
}