using System;
using TypeScriptModel.Expressions;
using TypeScriptModel.ExtensionMethods;

namespace TypeScriptModel.Statements
{
    [Serializable]
	public class JsVariableDeclaration {
		public string Name { get; private set; }
		/// <summary>
		/// Null if the variable is not initialized.
		/// </summary>
		public JsExpression Initializer { get; private set; }

		public JsVariableDeclaration(string name, JsExpression initializer) {
			if (name == null) throw new ArgumentNullException("name");
			if (!name.IsValidJavaScriptIdentifier()) throw new ArgumentException("name");
			Name        = name;
			Initializer = initializer;
		}
	}
}