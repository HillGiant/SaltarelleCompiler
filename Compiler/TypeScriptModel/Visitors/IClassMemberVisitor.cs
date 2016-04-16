using TypeScriptModel.Elements.ClassMembers;

namespace TypeScriptModel.Visitors
{
    public interface IClassMemberVisitor<out TReturn, in TData>
    {
        TReturn VisitConstructorDeclaration(TsConstructorDeclaration tsConstructorDeclaration, TData data);

        TReturn VisitClassConstructorSignature(TsClassConstructorSignature tsClassConstructorSignature, TData data);

        TReturn VisitMethodDeclaration(TsMethodDeclaration tsMethodDeclaration, TData data);

        TReturn VisitClassMethodSignature(TsClassMethodSignature tsClassMethodSignature, TData data);

        TReturn VisitClassIndexSignature(TsClassIndexSignature tsClassIndexSignaure, TData data);

        TReturn VisitClassSetAccessor(TsClassSetAccessor tsClassSetAccessor, TData data);

        TReturn VisitClassGetAccessor(TsClassGetAccessor tsClassGetAccessor, TData data);
    }
}
