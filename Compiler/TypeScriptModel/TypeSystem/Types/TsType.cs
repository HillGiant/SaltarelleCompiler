namespace TypeScriptModel.TypeSystem.Types
{
    using TypeScriptModel.Visitors;

    public abstract class TsType
    {
        public abstract TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data);
    }
}