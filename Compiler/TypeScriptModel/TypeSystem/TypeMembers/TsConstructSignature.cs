namespace TypeScriptModel.TypeSystem.TypeMembers
{
    using System.Collections.Generic;

    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.Types;
    using TypeScriptModel.Visitors;

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
            this.ReturnType = returnType;
            this.Parameters = parameters;
            this.TypeParameters = typeParameters;
        }

        public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitConstructSignature(this, data);
        }
    }
}