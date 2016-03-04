using System;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsWhileStatement : JsStatement {
        public JsExpression Condition { get; private set; }
        public JsBlockStatement Body { get; private set; }

        public JsWhileStatement(JsExpression condition, JsStatement body) {
            if (condition == null) throw new ArgumentNullException("condition");
            if (body == null) throw new ArgumentNullException("body");

            Condition = condition;
            Body      = JsBlockStatement.MakeBlock(body);
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitWhileStatement(this, data);
        }
    }
}
