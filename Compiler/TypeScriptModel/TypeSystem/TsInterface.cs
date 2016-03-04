using System.Collections.Generic;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem {
    public class TsInterface : TsCompositeType {
		public string Name { get; private set; }
        public IList<TsTypeReference> Extends { get; private set; }

		public TsInterface(string name, IEnumerable<TsTypeReference> extends, IEnumerable<TsMember> members) 
            :base(members)
        {
			Name = name;
			Extends = new List<TsTypeReference>(extends).AsReadOnly();
		}

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitInterface(this, data);
        }
	}
}