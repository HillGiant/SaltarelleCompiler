using System;
using TypeScriptModel.ExtensionMethods;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsContinueStatement : JsStatement {
        /// <summary>
        /// Can be null if the statement does not have a target label.
        /// </summary>
        public string TargetLabel { get; private set; }

        public JsContinueStatement(string targetLabel = null) {
            if (targetLabel != null && !targetLabel.IsValidJavaScriptIdentifier()) throw new ArgumentException("targetLabel");
            TargetLabel = targetLabel;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitContinueStatement(this, data);
        }
    }
}
