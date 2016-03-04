using System;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsLabelledStatement : JsStatement {
        public string Label { get; private set; }
		public JsStatement Statement { get; private set; }

        public JsLabelledStatement(string label, JsStatement statement) {
			Require.ValidJavaScriptIdentifier(label, "label", allowNull: false);
			Label = label;
			Statement = statement;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitLabelledStatement(this, data);
        }
    }
}
