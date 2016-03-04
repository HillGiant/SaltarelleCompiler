using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsTypeReference : TsType {
		public string Name { get; private set; }

		public TsTypeReference(string name) {
			Name = name;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitTypeReference(this, data);
	    }
    }
}
