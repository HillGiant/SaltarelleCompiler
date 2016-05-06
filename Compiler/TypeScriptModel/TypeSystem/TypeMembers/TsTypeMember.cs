namespace TypeScriptModel.TypeSystem.TypeMembers
{
    using TypeScriptModel.Visitors;

    public abstract class TsTypeMember
    {
        public abstract TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data);
	}
}