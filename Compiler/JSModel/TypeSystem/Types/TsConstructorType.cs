using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem
{
    using TypeScriptModel.Visitors;

    public class TsConstructorType : TsType, IHasCallSignature
    {
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsParameter> Parameters { get; private set; }
        public TsType ReturnType { get; private set; }

        public TsConstructorType(IList<TsTypeParameter> typeParameters, IList<TsParameter> parameters, TsType returnType)
        {
            TypeParameters = typeParameters;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitConstructorType(this, data);
        }
    }
}