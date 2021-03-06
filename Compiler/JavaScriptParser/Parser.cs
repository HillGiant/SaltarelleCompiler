﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Xebic.Parsers.ES3;
using Saltarelle.Compiler.JSModel.Expressions;
using Saltarelle.Compiler.JSModel.Statements;
using TypeScriptModel.Elements;
using TypeScriptModel.TypeSystem;

namespace JavaScriptParser {
	public static class Parser {
		public static JsExpression ParseExpression(string source, bool allowCustomKeywords = false) {
			var lex = new ES3Lexer(new ANTLRStringStream(source)) { AllowCustomKeywords = allowCustomKeywords };
			var tokens = new CommonTokenStream(lex);
			var parser = new ES3Parser(tokens);

			var r = parser.expressionOnly();
			var tree = new ES3Walker(new CommonTreeNodeStream(r.Tree));
			return tree.expression();
		}

		public static JsStatement ParseStatement(string source, bool allowCustomKeywords = false) {
			var lex = new ES3Lexer(new ANTLRStringStream(source.Trim())) { AllowCustomKeywords = allowCustomKeywords };
			var tokens = new CommonTokenStream(lex);
			var parser = new ES3Parser(tokens);

			var r = parser.sourceElement();
			var tree = new ES3Walker(new CommonTreeNodeStream(r.Tree));
			return tree.statement();
		}

                public static IList<TsSourceElement> ParseProgram(string source)
                {
			var lex = new ES3Lexer(new ANTLRStringStream(source));
   			var tokens = new CommonTokenStream(lex);
			var parser = new ES3Parser(tokens);

			var r = parser.program();
			var tree = new ES3Walker(new CommonTreeNodeStream(r.Tree));
			return tree.program();
                }

                public static TsType ParseType(string source)
                {
                    var lex = new ES3Lexer(new ANTLRStringStream(source));
                    CommonTokenStream tokens = new CommonTokenStream(lex);
                    var parser = new ES3Parser(tokens);

                    var r = parser.type();
                    if (r.Tree == null)
                        return null;
                    var tree = new ES3Walker(new CommonTreeNodeStream(r.Tree));
                    return tree.type();
                }

                public static TsSourceElement ParseSourceElement(string source)
                {
                    var lex = new ES3Lexer(new ANTLRStringStream(source));
                    CommonTokenStream tokens = new CommonTokenStream(lex);
                    var parser = new ES3Parser(tokens);

                    var r = parser.sourceElement();
                    if (r.Tree == null)
                        return null;
                    var tree = new ES3Walker(new CommonTreeNodeStream(r.Tree));
                    return tree.sourceElement();
                }
	}
}
