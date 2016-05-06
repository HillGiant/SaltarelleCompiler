using Antlr.Runtime;
using Antlr.Runtime.Tree;
using System.Collections.Generic;
using TypeScriptModel.Statements;
using TypeScriptModel.TypeSystem.Parameters;
using TypeScriptModel.Elements;
using TypeScriptModel.Expressions;

namespace TypeScriptParser
{
    using TypeScriptModel.TypeSystem.Types;

    public static class Parser
    {
        public static JsExpression ParseExpression(string source)
        {
            var lex = new TypeScriptParserImpl.TypeScriptLexer(new ANTLRStringStream(source));
            CommonTokenStream tokens = new CommonTokenStream(lex);
            var parser = new TypeScriptParserImpl.TypeScriptParser(tokens);

            var r = parser.expression();
            var tree = new TypeScriptParserImpl.TypeScriptWalker(new CommonTreeNodeStream(r.Tree));
            return tree.expression();
        }

        public static JsStatement ParseStatement(string source)
        {
            var lex = new TypeScriptParserImpl.TypeScriptLexer(new ANTLRStringStream(source.Trim()));
            CommonTokenStream tokens = new CommonTokenStream(lex);
            var parser = new TypeScriptParserImpl.TypeScriptParser(tokens);

            var r = parser.sourceElement();
            var tree = new TypeScriptParserImpl.TypeScriptWalker(new CommonTreeNodeStream(r.Tree));
            return tree.statement();
        }

        public static IList<TsSourceElement> Parse(string source, IErrorReporter errorReporter)
        {
            var lex = new TypeScriptParserImpl.TypeScriptLexer(new ANTLRStringStream(source)) { ErrorReporter = errorReporter };
            CommonTokenStream tokens = new CommonTokenStream(lex);
            var parser = new TypeScriptParserImpl.TypeScriptParser(tokens) { ErrorReporter = errorReporter };

            var r = parser.program();
            if (r.Tree == null)
                return new List<TsSourceElement>();//new TsGlobals(new TsModule[0], new TsInterface[0], null);
            var tree = new TypeScriptParserImpl.TypeScriptWalker(new CommonTreeNodeStream(r.Tree)) { ErrorReporter = errorReporter };
            return tree.program();
        }

        public static TsType ParseType(string source, IErrorReporter errorReporter)
        {
            var lex = new TypeScriptParserImpl.TypeScriptLexer(new ANTLRStringStream(source)) { ErrorReporter = errorReporter };
            CommonTokenStream tokens = new CommonTokenStream(lex);
            var parser = new TypeScriptParserImpl.TypeScriptParser(tokens) { ErrorReporter = errorReporter };

            var r = parser.type();
            if (r.Tree == null)
                return null;
            var tree = new TypeScriptParserImpl.TypeScriptWalker(new CommonTreeNodeStream(r.Tree)) { ErrorReporter = errorReporter };
            return tree.type();
        }

        public static TsSourceElement ParseSourceElement(string source, IErrorReporter errorReporter)
        {
            var lex = new TypeScriptParserImpl.TypeScriptLexer(new ANTLRStringStream(source)) { ErrorReporter = errorReporter };
            CommonTokenStream tokens = new CommonTokenStream(lex);
            var parser = new TypeScriptParserImpl.TypeScriptParser(tokens) { ErrorReporter = errorReporter };

            var r = parser.sourceElement();
            if (r.Tree == null)
                return null;
            var tree = new TypeScriptParserImpl.TypeScriptWalker(new CommonTreeNodeStream(r.Tree)) { ErrorReporter = errorReporter };
            return tree.sourceElement();
        }
    }
}
