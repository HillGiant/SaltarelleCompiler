namespace TypeScriptModel.TypeSystem.TypeMembers
{
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.Types;
    using TypeScriptModel.Visitors;

    public class TsIndexSignature : TsTypeMember {
		public TsType ReturnType { get; private set; }
		public string ParameterName { get; private set; }
                public TsPrimitiveType ParameterType { get; private set; }

		public TsIndexSignature(string parameterName, TsPrimitiveType parameterType, TsType returnType) {
			this.ReturnType    = returnType;
			this.ParameterName = parameterName;
			this.ParameterType = parameterType;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitIndexSignature(this, data);
	    }
    }
}