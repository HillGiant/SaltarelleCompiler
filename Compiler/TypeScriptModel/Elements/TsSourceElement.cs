using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.Elements
{
    public interface TsSourceElement
    {
        TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data);
    }
}
