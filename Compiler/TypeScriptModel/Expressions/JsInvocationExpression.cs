using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TypeScriptModel.ExtensionMethods;

namespace TypeScriptModel.Expressions {
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsInvocationExpression : JsExpression {
        public JsExpression Method { get; private set; }
        public ReadOnlyCollection<JsExpression> Arguments { get; private set; }

        internal JsInvocationExpression(JsExpression method, IEnumerable<JsExpression> arguments) : base(ExpressionNodeType.Invocation) {
            if (method == null) throw new ArgumentNullException("method");
            if (arguments == null) throw new ArgumentNullException("arguments");
            Method = method;
            Arguments = arguments.AsReadOnly();
			if (Arguments.Any(a => a == null)) throw new ArgumentException("All arguments must be non-null", "arguments");
        }

        public override TReturn Accept<TReturn, TData>(IExpressionVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitInvocationExpression(this, data);
        }
    }
}
