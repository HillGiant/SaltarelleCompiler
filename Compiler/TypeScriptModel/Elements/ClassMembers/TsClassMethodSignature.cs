namespace TypeScriptModel.Elements.ClassMembers
{
    using TypeScriptModel.Visitors;
    using System.Collections.Generic;

    using TypeScriptModel.TypeSystem;
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.Types;

    public class TsClassMethodSignature : TsClassMember, IHasCallSignature
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

        public override TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClassMethodSignature(this, data);
        }
    }
}
