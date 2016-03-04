using System;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsExpressionStatement : JsStatement {
        public JsExpression Expression { get; private set; }

        public JsExpressionStatement(JsExpression expression) {
            if (expression == null) throw new ArgumentNullException("expression");
            Expression = expression;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitExpressionStatement(this, data);
        }
    }
}
