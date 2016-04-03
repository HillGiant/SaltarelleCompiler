using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem
{
    using TypeScriptModel.Visitors;

    public class TsMethodSignature : TsTypeMember, IHasCallSignature
    {
        public string Name { get; private set; }
        public bool Optional { get; private set; }

        public TsType ReturnType { get; private set; }

        public IList<TsParameter> Parameters { get; private set; }

        public IList<TsTypeParameter> TypeParameters { get; private set; }

        public TsMethodSignature(
            string name,
            bool optional,
            IList<TsTypeParameter> typeParameters,
            IList<TsParameter> parameters,
            TsType returnType)
        {
            Name = name;
            Optional = optional;
            TypeParameters = typeParameters;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitMethodSignature(this, data);
        }
    }
}
