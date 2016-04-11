using System.Collections.Generic;

namespace TypeScriptModel.Elements.ClassMembers
{
    using TypeScriptModel.Statements;
    using TypeScriptModel.TypeSystem;
    using TypeScriptModel.Visitors;

    public class TsConstructorDeclaration: TsClassMember
    {
        public List<TsCallSignature> Signatures { get; private set; }
        public JsBlockStatement Body { get; private set; }

        public TsConstructorDeclaration(List<TsCallSignature> signatures, JsBlockStatement body)
        {
            Signatures = signatures;
            Body = body;
        }

        public override TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitConstructorDeclaration(this, data);
        }
    }
}
