using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    using System.Collections.Generic;

    public class TsTypeReference : TsType {
		public string Name { get; private set; }
        public IList<TsType> TypeArgs { get; private set; }

		public TsTypeReference(string name, IList<TsType> typeArgs ) {
			Name = name;
		    TypeArgs = typeArgs;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitTypeReference(this, data);
	    }
    }
}
