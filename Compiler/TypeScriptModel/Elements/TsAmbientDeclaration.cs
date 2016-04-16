namespace TypeScriptModel.Elements
{
    using TypeScriptModel.Visitors;

    public class TsAmbientDeclaration : TsSourceElement
    {
        public TsSourceElement Declared;

        public TsAmbientDeclaration(TsSourceElement declared)
        {
            this.Declared = declared;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitAmbientDeclaration(this, data);
        }
    }
}
