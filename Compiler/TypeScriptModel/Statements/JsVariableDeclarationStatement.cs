using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TypeScriptModel.Expressions;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.ExtensionMethods;
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsVariableDeclarationStatement : JsStatement {
        public ReadOnlyCollection<JsVariableDeclaration> Declarations { get; private set; }

        public JsVariableDeclarationStatement(IEnumerable<JsVariableDeclaration> declarations) {
            if (declarations == null) throw new ArgumentNullException("declarations");
            Declarations = declarations.AsReadOnly();
        }

        public JsVariableDeclarationStatement(params JsVariableDeclaration[] declarations) : this((IEnumerable<JsVariableDeclaration>)declarations) {
        }

        public JsVariableDeclarationStatement(string name, JsExpression initializer) : this(new JsVariableDeclaration(name, initializer)) {
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitVariableDeclarationStatement(this, data);
        }
    }
}
