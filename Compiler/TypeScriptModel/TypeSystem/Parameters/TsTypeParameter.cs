namespace TypeScriptModel.TypeSystem.Parameters
{
    using TypeScriptModel.TypeSystem.Types;

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