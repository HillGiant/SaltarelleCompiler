using System.Collections.Generic;

namespace TypeScriptModel.Elements.ClassMembers
{
    using Saltarelle.Compiler.JSModel.Statements;
    using TypeScriptModel.Visitors;

    public class TsConstructorDeclaration: TsClassMember
    {
        public IList<TsClassConstructorSignature> Signatures { get; private set; }
        public JsStatement Body { get; private set; }

        public TsConstructorDeclaration(IList<TsClassConstructorSignature> signatures, JsStatement body)
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
