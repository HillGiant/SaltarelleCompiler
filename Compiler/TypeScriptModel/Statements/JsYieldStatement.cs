using System;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsYieldStatement : JsStatement {
		/// <summary>
		/// Value to yield. If this is null (as opposed to JsExpression.Null), the yield terminates the iterator block (C#: 'yield break').
		/// </summary>
        public JsExpression Value { get; private set; }

        public JsYieldStatement(JsExpression value) {
            this.Value = value;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitYieldStatement(this, data);
        }
    }
}
