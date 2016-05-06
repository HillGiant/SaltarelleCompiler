namespace TypeScriptModel.TypeSystem.Types
{
    using System.Collections.Generic;

    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.Visitors;

    public class TsConstructorType : TsType, IHasCallSignature
    {
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsParameter> Parameters { get; private set; }
        public TsType ReturnType { get; private set; }

        public TsConstructorType(IList<TsTypeParameter> typeParameters, IList<TsParameter> parameters, TsType returnType)
        {
            this.TypeParameters = typeParameters;
            this.Parameters = parameters;
            this.ReturnType = returnType;
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitConstructorType(this, data);
        }
    }
}