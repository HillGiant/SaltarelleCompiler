using TypeScriptModel.Statements;
using TypeScriptModel.TypeSystem;
using TypeScriptModel.TypeSystem.Parameters;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.Elements.ClassMembers
{
    using TypeScriptModel.TypeSystem.Types;

    public class TsClassGetAccessor : TsClassMember
    {
        public AccessibilityModifier? Accessibility;

        public bool IsStatic;

        public string Name { get; private set; }

        public TsType TypeAnnotation { get; private set; }

        public JsStatement Body { get; private set; }

        public TsClassGetAccessor(AccessibilityModifier? accessibility, bool isStatic, string name, TsType typeAnnotation, JsStatement body)
        {
            Accessibility = accessibility;
            IsStatic = isStatic;
            Name = name;
            TypeAnnotation = typeAnnotation;
            Body = body;
        }

        public override TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClassGetAccessor(this, data);
        }
    }
}
