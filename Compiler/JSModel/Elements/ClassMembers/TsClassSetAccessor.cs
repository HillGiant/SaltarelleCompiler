﻿namespace TypeScriptModel.Elements.ClassMembers
{
    using Saltarelle.Compiler.JSModel.Statements;
    using TypeScriptModel.TypeSystem;
    using TypeScriptModel.Visitors;

    public class TsClassSetAccessor : TsClassMember
    {
        public AccessibilityModifier? Accessibility;

        public bool IsStatic;

        public string Name { get; private set; }

        public TsType TypeAnnotation { get; private set; }

        public TsParameter Parameter { get; private set; }

        public JsStatement Body { get; private set; }

        public TsClassSetAccessor(AccessibilityModifier? accessibility, bool isStatic, string name, TsParameter parameter, TsType typeAnnotation, JsStatement body)
        {
            Accessibility = accessibility;
            IsStatic = isStatic;
            Name = name;
            Parameter = parameter;
            TypeAnnotation = typeAnnotation;
            Body = body;
        }

        public override TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClassSetAccessor(this, data);
        }
    }
}
