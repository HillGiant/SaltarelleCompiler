using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TypeScriptModel.Expressions;
using TypeScriptModel.ExtensionMethods;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsSwitchStatement : JsStatement {
        public JsExpression Expression { get; private set; }
        public ReadOnlyCollection<JsSwitchSection> Sections { get; private set; }

        public JsSwitchStatement(JsExpression expression, IEnumerable<JsSwitchSection> sections) {
            if (expression == null) throw new ArgumentNullException("expression");
            if (sections == null) throw new ArgumentNullException("sections");

            Expression = expression;
            Sections   = sections.AsReadOnly();

            if (Sections.SelectMany(c => c.Values).Count(v => v == null) > 1) throw new ArgumentException("Can only have one default clause", "sections");
        }

        public JsSwitchStatement(JsExpression expression, params JsSwitchSection[] clauses) : this(expression, (IEnumerable<JsSwitchSection>)clauses) {
		}

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitSwitchStatement(this, data);
        }
    }
}
