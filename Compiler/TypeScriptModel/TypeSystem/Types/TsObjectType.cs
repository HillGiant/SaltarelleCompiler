using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem
{
    using TypeScriptModel.Visitors;

    public class TsObjectType : TsType
    {
        public IList<TsTypeMember> Members { get; private set; }

        public TsObjectType(IEnumerable<TsTypeMember> members)
        {
            Members = new List<TsTypeMember>(members).AsReadOnly();
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitObjectType(this, data);
        }
    }
}