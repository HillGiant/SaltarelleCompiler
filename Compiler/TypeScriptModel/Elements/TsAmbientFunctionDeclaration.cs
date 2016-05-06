namespace TypeScriptModel.Elements
{
    using System.Collections.Generic;

    using TypeScriptModel.TypeSystem;
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.Types;
    using TypeScriptModel.Visitors;

    public class TsAmbientFunctionDeclaration : TsSourceElement, IHasCallSignature
    {
        public string Name { get; private set; }

        public bool Optional { get; private set; }

        public TsType ReturnType { get; private set; }

        public IList<TsParameter> Parameters { get; private set; }

        public IList<TsTypeParameter> TypeParameters { get; private set; }

        public TsAmbientFunctionDeclaration(
            string name,
            bool optional,
            IList<TsTypeParameter> typeParameters,
            IList<TsParameter> parameters,
            TsType returnType)
        {
            this.Name = name;
            this.Optional = optional;
            this.TypeParameters = typeParameters;
            this.Parameters = parameters;
            this.ReturnType = returnType;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitAmbientFunctionDeclaration(this, data);
        }
    }
}
