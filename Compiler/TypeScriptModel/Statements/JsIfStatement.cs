﻿using System;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsIfStatement : JsStatement {
        public JsExpression Test { get; private set; }
        public JsBlockStatement Then { get; private set; }
        /// <summary>
        /// Can be null if there is no else clause.
        /// </summary>
        public JsBlockStatement Else { get; private set; }

        public JsIfStatement(JsExpression test, JsStatement then, JsStatement @else) {
            if (test == null) throw new ArgumentNullException("test");
            if (then == null) throw new ArgumentNullException("then");
            Test = test;
            Then = JsBlockStatement.MakeBlock(then);
            Else = JsBlockStatement.MakeBlock(@else);
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitIfStatement(this, data);
        }
    }
}
