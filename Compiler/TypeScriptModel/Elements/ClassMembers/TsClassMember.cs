namespace TypeScriptModel.Elements.ClassMembers
{
    using TypeScriptModel.Visitors;

    public abstract class TsClassMember
    {
        public abstract TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data);
    }
}
