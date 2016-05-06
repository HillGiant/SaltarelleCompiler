namespace TypeScriptModel.TypeSystem.Types
{
    using System.Collections.Generic;

    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.Visitors;

    public class TsTupleType : TsType
    {
        public IList<TsType> Types { get; private set; }

        public TsTupleType(IList<TsType> types)
        {
            this.Types = types;
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitTupleType(this, data);
        }
    }
}
