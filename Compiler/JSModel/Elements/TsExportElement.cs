namespace TypeScriptModel.Elements
{
    using TypeScriptModel.Visitors;

    public class TsExportElement: TsSourceElement
    {
        public TsSourceElement Exported;

        public TsExportElement(TsSourceElement exported)
        {
            this.Exported = exported;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitExport(this, data);
        }
    }
}
