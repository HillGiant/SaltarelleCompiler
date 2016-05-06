using TypeScriptModel.Visitors;

namespace TypeScriptModel.Elements
{
    public interface TsSourceElement
    {
        TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data);
    }
}
