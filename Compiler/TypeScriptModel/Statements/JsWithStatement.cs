using System;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsWithStatement : JsStatement {
        public JsExpression Object { get; private set; }
        public JsStatement Body { get; private set; }

        public JsWithStatement(JsExpression @object, JsStatement body) {
            if (@object == null) throw new ArgumentNullException("object");
            if (body == null) throw new ArgumentNullException("body");
            Object = @object;
            Body   = body;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitWithStatement(this, data);
        }
    }
}
