using System.Collections.Generic;
using TypeScriptModel.Visitors;
using TypeScriptModel.TypeSystem;

namespace TypeScriptModel.Elements
{
    public class TsInterface: TsSourceElement
    {
        public string Name { get; private set; }

        public IList<TsTypeReference> Extends { get; private set; }

        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsTypeMember> Members { get; private set; }

        public TsInterface(
            string name,
            IList<TsTypeParameter> typeParameters,
            IList<TsTypeReference> extends,
            IList<TsTypeMember> members)
        {
            Name = name;
            Extends = extends;
            TypeParameters = typeParameters;
            Members = members;

        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitInterface(this, data);
        }
    }
}