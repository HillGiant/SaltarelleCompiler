using TypeScriptModel.Statements;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.Elements.ClassMembers
{
    using TypeScriptModel.TypeSystem;

    public class TsClassMemberDeclaration : TsClassMember
    {
        public AccessibilityModifier? Accessibility;

        public bool IsStatic;

        public JsVariableDeclaration VariableDeclaration;

        public TsClassMemberDeclaration(AccessibilityModifier? accessibility, bool isStatic, JsVariableDeclaration variableDeclaration)
        {
            Accessibility = accessibility;
            IsStatic = isStatic;
            VariableDeclaration = variableDeclaration;
        }

        public override TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClassMemberDeclaration(this, data);
        }
    }
}
