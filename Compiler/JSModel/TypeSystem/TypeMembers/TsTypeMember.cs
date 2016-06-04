using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public abstract class TsTypeMember
    {
        public abstract TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data);
	}
}