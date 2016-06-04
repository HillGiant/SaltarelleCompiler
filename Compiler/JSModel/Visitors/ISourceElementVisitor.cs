using TypeScriptModel.Elements;

namespace TypeScriptModel.Visitors
{
    using TypeScriptModel.TypeSystem.Elements;

    public interface ISourceElementVisitor<out TReturn, in TData>
    {
        TReturn VisitInterface(TsInterface tsInterface, TData data);

        TReturn VisitStatementElement(TsStatementElement statement, TData data);

        TReturn VisitClass(TsClass tsClass, TData data);

        TReturn VisitModule(TypeSystem.TsModule tsModule, TData data);

        TReturn VisitExport(TsExportElement tsExportElement, TData data);

        TReturn VisitAmbientDeclaration(TsAmbientDeclaration tsAmbientDeclaration, TData data);

        TReturn VisitAmbientFunctionDeclaration(TypeSystem.TsAmbientFunctionDeclaration tsAmbientFunctionDeclaration, TData data);

        TReturn VisitImportDeclaration(TsImportDeclaration tsImportDeclaration, TData data);
    }
}
