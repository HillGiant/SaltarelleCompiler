using System;
using Saltarelle.Compiler.JSModel.Expressions;
using Saltarelle.Compiler.JSModel.ExtensionMethods;
using TypeScriptModel.TypeSystem;

namespace Saltarelle.Compiler.JSModel.Statements {
    [Serializable]
    public class JsVariableDeclaration
    {
        public string Name { get; private set; }

        /// <summary>
        /// Null if the variable is not initialized.
        /// </summary>
        public JsExpression Initializer { get; private set; }

        public TsType Type { get; private set; }

        public JsVariableDeclaration(string name, JsExpression initializer, TsType type = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (!name.IsValidJavaScriptIdentifier()) throw new ArgumentException("name");
            Name = name;
            Initializer = initializer;
            Type = type;
        }
    }
}