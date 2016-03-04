using Antlr.Runtime;
using Antlr.Runtime.Tree;
using System.Collections.Generic;
using TypeScriptModel.Statements;
using TypeScriptModel.TypeSystem;

namespace TypeScriptParser {
	public static class Parser {
		public static IList<JsStatement> Parse(string source, IErrorReporter errorReporter) {
			var lex = new TypeScriptParserImpl.TypeScriptLexer(new ANTLRStringStream(source)) { ErrorReporter = errorReporter };
			CommonTokenStream tokens = new CommonTokenStream(lex);
			var parser = new TypeScriptParserImpl.TypeScriptParser(tokens) { ErrorReporter = errorReporter };

			var r = parser.program();
			if (r.Tree == null)
				return new List<JsStatement> ();//new TsGlobals(new TsModule[0], new TsInterface[0], null);
			var tree = new TypeScriptParserImpl.TypeScriptWalker(new CommonTreeNodeStream(r.Tree)) { ErrorReporter = errorReporter };
			return tree.program();
		}
	}
}
