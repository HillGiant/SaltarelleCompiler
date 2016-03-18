using System.Collections.Generic;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.TypeSystem
{
    public class TsConstructorSignature : TsTypeMember, IHasCallSignature
    {
        public TsType ReturnType { get; private set; }

        public IList<TsParameter> Parameters { get; private set; }

        public IList<TsTypeParameter> TypeParameters { get; private set; }

        public TsConstructorSignature(
            TsType returnType,
            IEnumerable<TsTypeParameter> typeParameters,
            IEnumerable<TsParameter> parameters)
        {
            ReturnType = returnType;
            Parameters = new List<TsParameter>(parameters).AsReadOnly();
            TypeParameters = new List<TsTypeParameter>(typeParameters).AsReadOnly();
        }

        public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitConstructorSignature(this, data);
        }
    }
}