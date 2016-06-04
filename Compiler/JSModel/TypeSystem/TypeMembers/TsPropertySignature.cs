using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsPropertySignature : TsTypeMember {
		public string Name { get; private set; }
		public TsType Type { get; private set; }
		public bool Optional { get; private set; }

		public TsPropertySignature(string name, TsType type, bool optional) {
			Name     = name;
			Type     = type;
			Optional = optional;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitPropertySignature(this, data);
	    }
    }
}