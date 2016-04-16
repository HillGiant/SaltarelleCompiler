namespace TypeScriptModel.Elements.ClassMembers
{
    using TypeScriptModel.Visitors;
    using System.Collections.Generic;

    using TypeScriptModel.TypeSystem;

    public class TsClassMethodSignature : IHasCallSignature
    {
        public AccessibilityModifier? Accessibility;

        public bool IsStatic;

        public string Name;

        public TsType ReturnType { get; private set; }

        public IList<TsTypeParameter> TypeParameters { get; private set; }

        public IList<TsParameter> Parameters { get; private set; }

        public TsClassMethodSignature(AccessibilityModifier? accessibility, bool isStatic, string name, IList<TsParameter> parameters, IList<TsTypeParameter> typeParameters, TsType returnType)
        {
            Accessibility = accessibility;
            IsStatic = isStatic;
            Name = name;
            Parameters = parameters;
            TypeParameters = typeParameters;
            ReturnType = returnType;
        }

        public TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClassMethodSignature(this, data);
        }
    }
}
