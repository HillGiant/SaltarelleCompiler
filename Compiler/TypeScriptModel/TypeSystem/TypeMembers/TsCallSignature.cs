using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem
{
    using TypeScriptModel.Visitors;

    public class TsCallSignature : TsTypeMember, IHasCallSignature
    {
        public string Name { get; private set; }
        public TsType ReturnType { get; private set; }
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public IList<TsParameter> Parameters { get; private set; }

        public TsCallSignature(string name, IEnumerable<TsTypeParameter> typeParameters, IEnumerable<TsParameter> parameters, TsType returnType)
        {
            Name = name;
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