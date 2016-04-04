using TypeScriptModel.Elements;

namespace TypeScriptModel.Visitors
{
    public interface ISourceElementVisitor<out TReturn, in TData>
    {
        TReturn VisitInterface(TsInterface tsInterface, TData data);

        TReturn VisitStatementElement(TsStatementElement statement, TData data);
    }
}
