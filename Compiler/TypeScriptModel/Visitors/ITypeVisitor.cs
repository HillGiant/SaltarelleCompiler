using TypeScriptModel.TypeSystem;

namespace TypeScriptModel.Visitors
{
    public interface ITypeVisitor<out TReturn, in TData>
    {
        TReturn VisitArrayType(TsArrayType type, TData data);

        TReturn VisitObjectType(TsObjectType type, TData data);

        TReturn VisitFunctionType(TsFunctionType type, TData data);

        TReturn VisitTypeReference(TsTypeReference type, TData data);

        TReturn VisitInterfaceType(TsInterfaceType type, TData data);

        TReturn VisitPrimitiveType(TsPrimitiveType tsPrimitiveType, TData data);
    }
}
