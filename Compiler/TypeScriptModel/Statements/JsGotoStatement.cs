using System;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsGotoStatement : JsStatement {
        public string TargetLabel { get; private set; }

        public JsGotoStatement(string targetLabel) {
			Require.ValidJavaScriptIdentifier(targetLabel, "targetLabel", allowNull: false);
			TargetLabel = targetLabel;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitGotoStatement(this, data);
        }
    }
}
