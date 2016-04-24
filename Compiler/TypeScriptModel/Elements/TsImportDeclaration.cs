using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.Elements
{
    public class TsImportDeclaration: TsSourceElement
    {
        public string Alias { get; set; }
        public string Module { get; set; }

        public TsImportDeclaration(string alias, string module)
        {
            Alias = alias;
            Module = module;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitImportDeclaration(this, data);
        }
    }
}
