using System;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
	public class JsAwaitStatement : JsStatement {
		public JsExpression Awaiter { get; private set; }
		public string OnCompletedMethodName { get; private set; }

		public JsAwaitStatement(JsExpression awaiter, string onCompletedMethodName) {
			if (awaiter == null)
				throw new ArgumentException("awaiter");
			Require.ValidJavaScriptIdentifier("onCompletedMethodName", onCompletedMethodName);
			Awaiter = awaiter;
			OnCompletedMethodName = onCompletedMethodName;
		}

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitAwaitStatement(this, data);
        }
	}
}
