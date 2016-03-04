using TypeScriptModel.TypeSystem;

namespace TypeScriptModel.Visitors
{
    public interface ITypeVisitor<out TReturn, in TData> 
    {
        TReturn VisitArrayType(TsArrayType type, TData data);
        TReturn VisitCompositeType(TsCompositeType type, TData data);
        TReturn VisitFunctionType(TsFunctionType type, TData data);
        TReturn VisitTypeReference(TsTypeReference type, TData data);
        TReturn VisitInterface(TsInterface type, TData data);
    }
}
