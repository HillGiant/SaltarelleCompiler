using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem {
    using TypeScriptModel.Visitors;

    public class TsFunctionType : TsType, IHasCallSignature
    {
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsParameter> Parameters { get; private set; }
		public TsType ReturnType { get; private set; }

        public TsFunctionType(IEnumerable<TsTypeParameter> typeParameters, IEnumerable<TsParameter> parameters, TsType returnType)
        {
            TypeParameters = new List<TsTypeParameter>(typeParameters).AsReadOnly();
			Parameters = new List<TsParameter>(parameters).AsReadOnly();
			ReturnType = returnType;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitFunctionType(this, data);
	    }
    }
}