using System.Collections.Generic;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsTupleType : TsType
    {
        public IList<TsType> Types { get; private set; }

        public TsTupleType(IEnumerable<TsType> types)
        {
            Types = new List<TsType>(types).AsReadOnly();
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitTupleType(this, data);
        }
    }
}
