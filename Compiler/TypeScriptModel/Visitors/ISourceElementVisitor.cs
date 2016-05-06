using TypeScriptModel.Elements;

namespace TypeScriptModel.Visitors
{
    public interface ISourceElementVisitor<out TReturn, in TData>
    {
        TReturn VisitInterface(TsInterface tsInterface, TData data);

        TReturn VisitStatementElement(TsStatementElement statement, TData data);

        TReturn VisitClass(TsClass tsClass, TData data);

        TReturn VisitModule(TsModule tsModule, TData data);

        TReturn VisitExport(TsExportElement tsExportElement, TData data);

        TReturn VisitAmbientDeclaration(TsAmbientDeclaration tsAmbientDeclaration, TData data);

        TReturn VisitAmbientFunctionDeclaration(TsAmbientFunctionDeclaration tsAmbientFunctionDeclaration, TData data);

        TReturn VisitImportDeclaration(TsImportDeclaration tsImportDeclaration, TData data);

        TReturn VisitNamespace(TsNamespace tsNamespace, TData data);
    }
}
