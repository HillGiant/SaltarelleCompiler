namespace TypeScriptModel.Elements.ClassMembers
{
    using Saltarelle.Compiler.JSModel.Statements;
    using System.Collections.Generic;
    using TypeScriptModel.Visitors;

    public class TsMethodDeclaration : TsClassMember
    {
        public IList<TsClassMethodSignature> Signatures { get; private set; }
        public JsStatement Body { get; private set; }

        public TsMethodDeclaration(IList<TsClassMethodSignature> signatures, JsStatement body)
        {
            Signatures = signatures;
            Body = body;
        }

        public override TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitMethodDeclaration(this, data);
        }
    }
}
