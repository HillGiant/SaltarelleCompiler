using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsIndexer : TsMember {
		public TsType ReturnType { get; private set; }
		public string ParameterName { get; private set; }
		public TsType ParameterType { get; private set; }

		public TsIndexer(TsType returnType, string parameterName, TsType parameterType) {
			ReturnType    = returnType;
			ParameterName = parameterName;
			ParameterType = parameterType;
		}

	    public override TReturn Accept<TReturn, TData>(IMemberVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitIndexer(this, data);
	    }
    }
}