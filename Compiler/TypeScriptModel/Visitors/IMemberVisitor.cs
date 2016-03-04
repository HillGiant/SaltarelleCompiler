using TypeScriptModel.TypeSystem;

namespace TypeScriptModel.Visitors
{
    public interface IMemberVisitor<out TReturn, in TData> 
    {
        TReturn VisitFunction(TsFunction member, TData data);
        TReturn VisitConstructor(TsConstructor member, TData data);
        TReturn VisitIndexer(TsIndexer member, TData data);
        TReturn VisitVariable(TsVariable member, TData data);
    }
}
