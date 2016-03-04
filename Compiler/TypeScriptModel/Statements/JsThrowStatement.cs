using System;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsThrowStatement : JsStatement
    {
        public JsExpression Expression { get; private set; }

        public JsThrowStatement(JsExpression expression) {
            this.Expression = expression;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitThrowStatement(this, data);
        }
    }
}
