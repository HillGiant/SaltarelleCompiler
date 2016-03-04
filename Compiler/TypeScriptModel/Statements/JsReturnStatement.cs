using System;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsReturnStatement : JsStatement {
        /// <summary>
        /// Can be null if the statement does not return a value.
        /// </summary>
        public JsExpression Value { get; private set; }

        public JsReturnStatement(JsExpression value = null) {
            Value = value;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitReturnStatement(this, data);
        }
    }
}
