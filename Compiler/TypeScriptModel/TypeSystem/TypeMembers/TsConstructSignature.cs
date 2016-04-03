using System.Collections.Generic;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsConstructSignature : TsTypeMember, IHasCallSignature
    {
        public TsType ReturnType { get; private set; }

        public IList<TsParameter> Parameters { get; private set; }

        public IList<TsTypeParameter> TypeParameters { get; private set; }

        public TsConstructSignature(
            IList<TsTypeParameter> typeParameters,
            IList<TsParameter> parameters,
            TsType returnType)
        {
            ReturnType = returnType;
            Parameters = parameters;
            TypeParameters = typeParameters;
        }

        public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitConstructSignature(this, data);
        }
    }
}