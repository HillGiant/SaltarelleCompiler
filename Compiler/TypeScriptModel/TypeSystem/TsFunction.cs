using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem {
    using TypeScriptModel.Visitors;

    public class TsFunction : TsMember {
		public string Name { get; private set; }
		public TsType ReturnType { get; private set; }
        public IList<TsParameter> Parameters { get; private set; }

		public TsFunction(string name, TsType returnType, IEnumerable<TsParameter> parameters) {
			Name = name;
			ReturnType = returnType;
			Parameters = new List<TsParameter>(parameters).AsReadOnly();
		}

	    public override TReturn Accept<TReturn, TData>(IMemberVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitFunction(this, data);
	    }
    }
}