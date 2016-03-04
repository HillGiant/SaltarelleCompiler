using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsVariable : TsMember {
		public string Name { get; private set; }
		public TsType Type { get; private set; }
		public bool Optional { get; private set; }

		public TsVariable(string name, TsType type, bool optional) {
			Name     = name;
			Type     = type;
			Optional = optional;
		}

	    public override TReturn Accept<TReturn, TData>(IMemberVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitVariable(this, data);
	    }
    }
}