using System;

namespace TypeScriptModel.Expressions {
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsBinaryExpression : JsExpression {
        public JsExpression Left { get; private set; }
        public JsExpression Right { get; private set; }

        internal JsBinaryExpression(ExpressionNodeType nodeType, JsExpression left, JsExpression right) : base(nodeType) {
            if (nodeType < ExpressionNodeType.BinaryFirst || nodeType > ExpressionNodeType.BinaryLast) throw new ArgumentException("nodeType");
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            Left  = left;
            Right = right;
        }

        public override string ToString() {
            return NodeType + " (" + Left + ", " + Right + ")";
        }

        public override TReturn Accept<TReturn, TData>(IExpressionVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitBinaryExpression(this, data);
        }
    }
}
