using System.Collections.Generic;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem {
    public class TsConstructor : TsMember {
		public TsType ReturnType { get; private set; }
        public IList<TsParameter> Parameters { get; private set; }

		public TsConstructor(TsType returnType, IEnumerable<TsParameter> parameters) {
			ReturnType = returnType;
			Parameters = new List<TsParameter>(parameters).AsReadOnly();
		}

	    public override TReturn Accept<TReturn, TData>(IMemberVisitor<TReturn, TData> visitor, TData data)
	    {
            return visitor.VisitConstructor(this, data);
	    }
    }
}