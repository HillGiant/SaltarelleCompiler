using TypeScriptModel.TypeSystem;

namespace TypeScriptModel.Visitors
{
    public interface ITypeMemberVisitor<out TReturn, in TData> 
    {
        TReturn VisitMethodSignature(TsMethodSignature member, TData data);
        TReturn VisitConstructorSignature(TsConstructorSignature member, TData data);
        TReturn VisitIndexSignature(TsIndexSignature member, TData data);
        TReturn VisitPropertySignature(TsPropertySignature member, TData data);
        TReturn VisitCallSignature(TsCallSignature tsCallSignature, TData data);
    }
}
