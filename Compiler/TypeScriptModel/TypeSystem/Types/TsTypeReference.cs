namespace TypeScriptModel.TypeSystem.Types
{
    using System.Collections.Generic;

    using TypeScriptModel.Visitors;

    public class TsTypeReference : TsType {
		public string Name { get; private set; }
        public IList<TsType> TypeArgs { get; private set; }

		public TsTypeReference(string name, IList<TsType> typeArgs ) {
			this.Name = name;
		    this.TypeArgs = typeArgs;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitTypeReference(this, data);
	    }
    }
}
