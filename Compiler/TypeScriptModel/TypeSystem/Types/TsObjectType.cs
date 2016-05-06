namespace TypeScriptModel.TypeSystem.Types
{
    using System.Collections.Generic;

    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.TypeMembers;
    using TypeScriptModel.Visitors;

    public class TsObjectType : TsType
    {
        public IList<TsTypeMember> Members { get; private set; }

        public TsObjectType(IEnumerable<TsTypeMember> members)
        {
            this.Members = new List<TsTypeMember>(members).AsReadOnly();
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitObjectType(this, data);
        }
    }
}