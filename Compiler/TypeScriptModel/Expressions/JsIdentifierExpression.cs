using System;
using TypeScriptModel.ExtensionMethods;

namespace TypeScriptModel.Expressions {
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsIdentifierExpression : JsExpression {
        public string Name { get; private set; }

        internal JsIdentifierExpression(string name) : base(ExpressionNodeType.Identifier) {
            if (name == null) throw new ArgumentNullException("name");
            if (!name.IsValidJavaScriptIdentifier()) throw new ArgumentException("name");
            Name = name;
        }

        public override string ToString() {
            return Name;
        }

        public override TReturn Accept<TReturn, TData>(IExpressionVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitIdentifierExpression(this, data);
        }
    }
}
