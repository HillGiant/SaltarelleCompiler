using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem
{
    using TypeScriptModel.Visitors;

    public class TsCallSignature : TsTypeMember, IHasCallSignature
    {
        public TsType ReturnType { get; private set; }
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsParameter> Parameters { get; private set; }

        public TsCallSignature(IList<TsTypeParameter> typeParameters, IList<TsParameter> parameters, TsType returnType)
        {
            ReturnType = returnType;
            Parameters = parameters;
            TypeParameters = typeParameters;
        }

        public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitCallSignature(this, data);
        }
    }
}