using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Saltarelle.Compiler.JSModel.Expressions;

namespace Saltarelle.Compiler.JSModel.OldTypeSystem {
	public class JsNamedConstructor {
		public string Name { get; private set; }
		public JsFunctionDefinitionExpression Definition { get; private set; }

		public JsNamedConstructor(string name, JsFunctionDefinitionExpression definition) {
			Name       = name;
			Definition = definition;
		}
	}
}
