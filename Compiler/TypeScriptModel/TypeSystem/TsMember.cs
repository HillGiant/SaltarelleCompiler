using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public abstract class TsMember
    {
        public abstract TReturn Accept<TReturn, TData>(IMemberVisitor<TReturn, TData> visitor, TData data);
	}
}