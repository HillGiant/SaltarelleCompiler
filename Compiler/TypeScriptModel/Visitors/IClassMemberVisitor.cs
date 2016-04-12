namespace TypeScriptModel.Visitors
{
    using TypeScriptModel.Elements.ClassMembers;

    public interface IClassMemberVisitor<out TReturn, in TData>
    {
        TReturn VisitConstructorDeclaration(TsConstructorDeclaration tsConstructorDeclaration, TData data);

        TReturn VisitClassConstructorSignature(TsClassConstructorSignature tsClassConstructorSignature, TData data);
    }
}
