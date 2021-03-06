﻿using System;
using System.Collections.Generic;
using TypeScriptModel.Visitors;
using System.Linq;
using System.Text;
using Saltarelle.Compiler.JSModel.Statements;

namespace TypeScriptModel.Elements
{
    public class TsStatementElement: TsSourceElement
    {        
        public JsStatement Statement { get; private set; }

        public TsStatementElement(JsStatement statement)
        {
            if (statement == null) throw new ArgumentNullException("statement");
            Statement = statement;
        }

        public TReturn Accept<TReturn, TData>(ISourceElementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitStatementElement(this, data);
        }
    }
}
