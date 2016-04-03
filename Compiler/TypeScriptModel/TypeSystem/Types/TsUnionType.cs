using System.Collections.Generic;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{

    public class TsUnionType : TsType
    {
        public IList<TsType> Types { get; private set; }

        public TsUnionType(IEnumerable<TsType> types)
        {
            Types = new List<TsType>(types).AsReadOnly();
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitUnionType(this, data);
        }
    }
}
