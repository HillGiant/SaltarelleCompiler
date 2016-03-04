using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem {
    using TypeScriptModel.Visitors;

    public class TsFunctionType : TsType {
        public IList<TsParameter> Parameters { get; private set; }
		public TsType ReturnType { get; private set; }

		public TsFunctionType(IEnumerable<TsParameter> parameters, TsType returnType) {
			Parameters = new List<TsParameter>(parameters).AsReadOnly();
			ReturnType = returnType;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitFunctionType(this, data);
	    }
    }
}