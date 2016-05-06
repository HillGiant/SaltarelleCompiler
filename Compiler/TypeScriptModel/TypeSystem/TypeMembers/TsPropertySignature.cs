namespace TypeScriptModel.TypeSystem.TypeMembers
{
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.Types;
    using TypeScriptModel.Visitors;

    public class TsPropertySignature : TsTypeMember {
		public string Name { get; private set; }
		public TsType Type { get; private set; }
		public bool Optional { get; private set; }

		public TsPropertySignature(string name, TsType type, bool optional) {
			this.Name     = name;
			this.Type     = type;
			this.Optional = optional;
		}

	    public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
	    {
	        return visitor.VisitPropertySignature(this, data);
	    }
    }
}