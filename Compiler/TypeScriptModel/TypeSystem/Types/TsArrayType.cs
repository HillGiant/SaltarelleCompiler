namespace TypeScriptModel.TypeSystem.Types {
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.Visitors;

    public class TsArrayType : TsType {
		public TsType ElementType { get; private set; }

		public TsArrayType(TsType elementType) {
			this.ElementType = elementType;
		}

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitArrayType(this, data);
	    }
    }
}