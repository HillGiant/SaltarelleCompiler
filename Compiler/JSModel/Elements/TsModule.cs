using System.Collections.Generic;
using TypeScriptModel.Elements;

namespace TypeScriptModel.TypeSystem
{
    using TypeScriptModel.Visitors;

    public class TsModule: TsSourceElement
    {
        public string Name { get; private set; }

        public IList<TsSourceElement> Elements { get; private set; }

        public TsModule(string name, IList<TsSourceElement> elements)
        {
            Name = name;
            Elements = elements;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitModule(this, data);
        }
    }
}