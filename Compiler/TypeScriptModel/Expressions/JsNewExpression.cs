using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TypeScriptModel.ExtensionMethods;

namespace TypeScriptModel.Expressions {
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsNewExpression : JsExpression {
        public JsExpression Constructor { get; private set; }
        public ReadOnlyCollection<JsExpression> Arguments { get; private set; }

        internal JsNewExpression(JsExpression constructor, IEnumerable<JsExpression> arguments) : base(ExpressionNodeType.New) {
            if (constructor == null) throw new ArgumentNullException("constructor");
            if (arguments == null) throw new ArgumentNullException("arguments");

            Constructor = constructor;
            Arguments = arguments.AsReadOnly();
        }

        public override TReturn Accept<TReturn, TData>(IExpressionVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitNewExpression(this, data);
        }
    }
}
