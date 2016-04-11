using TypeScriptModel.Elements;

namespace TypeScriptModel.Visitors
{
    using TypeScriptModel.TypeSystem.Elements;

    public interface ISourceElementVisitor<out TReturn, in TData>
    {
        TReturn VisitInterface(TsInterface tsInterface, TData data);

        TReturn VisitStatementElement(TsStatementElement statement, TData data);

        TReturn VisitClass(TsClass tsClass, TData data);
    }
}
