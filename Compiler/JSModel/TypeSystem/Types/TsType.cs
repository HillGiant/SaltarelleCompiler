using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public abstract class TsType
    {
        public abstract TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data);
    }
}