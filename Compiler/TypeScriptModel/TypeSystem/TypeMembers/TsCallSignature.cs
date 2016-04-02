using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem
{
    using TypeScriptModel.Visitors;

    public class TsCallSignature : TsTypeMember, IHasCallSignature
    {
        public TsType ReturnType { get; private set; }
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsParameter> Parameters { get; private set; }

        public TsCallSignature(IEnumerable<TsTypeParameter> typeParameters, IEnumerable<TsParameter> parameters, TsType returnType)
        {
            ReturnType = returnType;
            Parameters = new List<TsParameter>(parameters).AsReadOnly();
            TypeParameters = new List<TsTypeParameter>(typeParameters).AsReadOnly();
        }

        public override TReturn Accept<TReturn, TData>(ITypeMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitCallSignature(this, data);
        }
    }
}