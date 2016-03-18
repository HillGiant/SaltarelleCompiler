using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsIndexSignature : TsTypeMember {
		public TsType ReturnType { get; private set; }
		public string ParameterName { get; private set; }
		public TsType ParameterType { get; private set; }

		public TsIndexSignature(TsType returnType, string parameterName, TsType parameterType) {
			ReturnType    = returnType;
			ParameterName = parameterName;
			ParameterType = parameterType;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitIndexSignature(this, data);
	    }
    }
}