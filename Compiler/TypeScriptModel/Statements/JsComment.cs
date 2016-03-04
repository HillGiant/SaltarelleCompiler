using System;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsComment : JsStatement {
        public string Text { get; private set; }

        public JsComment(string text) {
            Text = text;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitComment(this, data);
        }
    }
}
