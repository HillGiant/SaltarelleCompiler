

namespace TypeScriptModel.Elements
{
    using System.Collections.Generic;

    using TypeScriptModel.Elements.ClassMembers;
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.Types;
    using TypeScriptModel.Visitors;

    public class TsClass : TsSourceElement
    {
        public string Name { get; private set; }
        public IList<TsTypeReference> Extends { get; private set; }
        public IList<TsTypeReference> Implements { get; private set; }
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsClassMember> Members { get; private set; }

        public TsClass(
            string name,
            IList<TsTypeParameter> typeParameters,
            IList<TsTypeReference> extends,
            IList<TsTypeReference> implements,
            IList<TsClassMember> members)
        {
            this.Name = name;
            this.Extends = extends;
            this.Implements = implements;
            this.TypeParameters = typeParameters;
            this.Members = members;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClass(this, data);
        }
    }
}
