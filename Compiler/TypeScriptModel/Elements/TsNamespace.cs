namespace TypeScriptModel.Elements
{
    using System.Collections.Generic;

    using TypeScriptModel.Visitors;

    public class TsNamespace : TsSourceElement
    {
        public string Name { get; private set; }

        public IList<TsSourceElement> Elements { get; private set; }

        public TsNamespace(string name, IList<TsSourceElement> elements)
        {
            this.Name = name;
            this.Elements = elements;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitNamespace(this, data);
        }
    }
}
