using System;

namespace TypeScriptModel.Expressions {
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsThisExpression : JsExpression {
        private JsThisExpression() : base(ExpressionNodeType.This) {
        }

		private static readonly JsThisExpression _instance = new JsThisExpression();
		internal new static JsThisExpression This { get { return _instance; } }

        public override TReturn Accept<TReturn, TData>(IExpressionVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitThisExpression(this, data);
        }
    }
}
