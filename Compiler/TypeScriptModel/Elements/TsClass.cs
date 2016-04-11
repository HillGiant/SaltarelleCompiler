

namespace TypeScriptModel.TypeSystem.Elements
{
    using System.Collections.Generic;
    using TypeScriptModel.Elements;
    using TypeScriptModel.Elements.ClassMembers;
    using TypeScriptModel.Visitors;

    public class TsClass : TsSourceElement
    {
        public string Name { get; private set; }
        public IList<TsTypeReference> Extends { get; private set; }
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsClassMember> Members { get; private set; }

        public TsClass(
            string name,
            IList<TsTypeParameter> typeParameters,
            IList<TsTypeReference> extends,
            IList<TsClassMember> members)
        {
            Name = name;
            Extends = extends;
            TypeParameters = typeParameters;
            Members = members;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClass(this, data);
        }
    }
}
