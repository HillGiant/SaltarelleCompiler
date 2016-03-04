using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem {
    using TypeScriptModel.Visitors;

    public class TsCompositeType : TsType {
        public IList<TsMember> Members { get; private set; }

		public TsCompositeType(IEnumerable<TsMember> members) {
			Members = new List<TsMember>(members).AsReadOnly();
		}

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitCompositeType(this, data);
        }
	}
}