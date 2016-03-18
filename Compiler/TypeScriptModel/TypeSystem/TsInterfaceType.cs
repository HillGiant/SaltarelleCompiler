using System.Collections.Generic;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsInterfaceType : TsObjectType
    {
        public string Name { get; private set; }

        public IList<TsTypeReference> Extends { get; private set; }

        public IList<TsTypeParameter> TypeParameters { get; private set; }

        public TsInterfaceType(
            string name,
            IEnumerable<TsTypeParameter> typeParameters,
            IEnumerable<TsTypeReference> extends,
            IEnumerable<TsTypeMember> members)
            : base(members)
        {
            Name = name;
            Extends = new List<TsTypeReference>(extends).AsReadOnly();
            TypeParameters = new List<TsTypeParameter>(typeParameters).AsReadOnly();
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitInterfaceType(this, data);
        }
    }
}