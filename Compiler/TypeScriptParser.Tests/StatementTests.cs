using System.Linq;
using NUnit.Framework;
using TypeScriptModel;
using Saltarelle.Compiler.JSModel.Statements;
using JavaScriptParser;
using Saltarelle.Compiler.JSModel.Expressions;

namespace Saltarelle.Compiler.Tests.JavaScriptParserTests
{
    [TestFixture]
    public class StatementTests
    {
        private T ParseStatement<T>(string source) where T : JsStatement
        {
            var stmt = Parser.ParseStatement(source);
            Assert.That(stmt, Is.InstanceOf<T>());
            return (T)stmt;
        }

        private void RoundtripStatement(string source, string expected = null)
        {
            var stmt = Parser.ParseStatement(source);
            Assert.That(OutputFormatter.Format(stmt).Replace("\r\n", "\n"), Is.EqualTo((expected ?? source).Replace("\r\n", "\n")));
        }

        [Test]
        public void ExpressionStatement()
        {
            var stmt = ParseStatement<JsExpressionStatement>("x;");
            Assert.That(stmt.Expression, Is.InstanceOf<JsIdentifierExpression>());
            Assert.That(((JsIdentifierExpression)stmt.Expression).Name, Is.EqualTo("x"));
        }

        [Test]
        public void BlockStatement()
        {
            var stmt = ParseStatement<JsBlockStatement>("{\n}");
            Assert.That(stmt.Statements, Is.Empty);

            stmt = ParseStatement<JsBlockStatement>("{x;}");
            Assert.That(stmt.Statements, Has.Count.EqualTo(1));
            Assert.That(stmt.Statements[0], Is.InstanceOf<JsExpressionStatement>());
        }

        [Test]
        public void EmptyStatement()
        {
            ParseStatement<JsEmptyStatement>(";");
        }

        [Test]
        public void LabelledStatement()
        {
            var stmt = ParseStatement<JsBlockStatement>("{lbl: x;}");
            Assert.That(stmt.Statements.Count, Is.EqualTo(1));
            Assert.That(stmt.Statements[0], Is.InstanceOf<JsLabelledStatement>());
            Assert.That(((JsLabelledStatement)stmt.Statements[0]).Label, Is.EqualTo("lbl"));
            Assert.That(((JsLabelledStatement)stmt.Statements[0]).Statement, Is.InstanceOf<JsExpressionStatement>());
            Assert.That(((JsExpressionStatement)((JsLabelledStatement)stmt.Statements[0]).Statement).Expression, Is.InstanceOf<JsIdentifierExpression>());
        }

        [Test]
        public void VariableDeclaration()
        {
            RoundtripStatement("var i;\n");
            RoundtripStatement("var i = 0;\n");
            RoundtripStatement("var i, j, k;\n");
            RoundtripStatement("var i = 0, j = 1, k = 2;\n");
            RoundtripStatement("var i = new foo(a, b, c);\n");
        }

        [Test]
        public void ArrowDeclaration()
        {
            RoundtripStatement("var legalize = d => d > 1 ? 1 : d;\n");
        }

        [Test]
        public void IfStatement()
        {
            var stmt = ParseStatement<JsIfStatement>("if (x) { y; } else { z; }");
            Assert.That(OutputFormatter.Format(stmt.Test), Is.EqualTo("x"));
            Assert.That(OutputFormatter.Format(stmt.Then).Replace("\r\n", "\n"), Is.EqualTo("{\n\ty;\n}\n"));
            Assert.That(OutputFormatter.Format(stmt.Else).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));

            stmt = ParseStatement<JsIfStatement>("if (x) { y; }");
            Assert.That(OutputFormatter.Format(stmt.Test), Is.EqualTo("x"));
            Assert.That(OutputFormatter.Format(stmt.Then).Replace("\r\n", "\n"), Is.EqualTo("{\n\ty;\n}\n"));
            Assert.That(stmt.Else, Is.Null);

            stmt = ParseStatement<JsIfStatement>("if (x) { y; } else if (z) { a; } else { b; }");
            Assert.That(OutputFormatter.Format(stmt.Test), Is.EqualTo("x"));
            Assert.That(OutputFormatter.Format(stmt.Then).Replace("\r\n", "\n"), Is.EqualTo("{\n\ty;\n}\n"));
            var jsElse = stmt.Else.Statements[0] as JsIfStatement;
            Assert.That(jsElse, Is.Not.Null);
            Assert.That(OutputFormatter.Format(jsElse.Test), Is.EqualTo("z"));
            Assert.That(OutputFormatter.Format(jsElse.Then).Replace("\r\n", "\n"), Is.EqualTo("{\n\ta;\n}\n"));
            Assert.That(OutputFormatter.Format(jsElse.Else).Replace("\r\n", "\n"), Is.EqualTo("{\n\tb;\n}\n"));
        }

        [Test]
        public void WhileStatement()
        {
            var stmt = ParseStatement<JsWhileStatement>("while (x) { y; }");
            Assert.That(OutputFormatter.Format(stmt.Condition), Is.EqualTo("x"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\ty;\n}\n"));
        }

        [Test]
        public void DoWhileStatement()
        {
            var stmt = ParseStatement<JsDoWhileStatement>("do { y; } while (x);");
            Assert.That(OutputFormatter.Format(stmt.Condition), Is.EqualTo("x"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\ty;\n}\n"));
        }

        [Test]
        public void ForStatement()
        {
            var stmt = ParseStatement<JsForStatement>("for (x = 0; x < y; x++) { z; }");
            Assert.That(stmt.InitStatement, Is.InstanceOf<JsExpressionStatement>());
            Assert.That(OutputFormatter.Format(stmt.InitStatement).Replace("\r\n", "\n"), Is.EqualTo("x = 0;\n"));
            Assert.That(OutputFormatter.Format(stmt.ConditionExpression), Is.EqualTo("x < y"));
            Assert.That(OutputFormatter.Format(stmt.IteratorExpression), Is.EqualTo("x++"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));

            stmt = ParseStatement<JsForStatement>("for (var x = 0, y = 0; x < y; x++) { z; }");
            Assert.That(stmt.InitStatement, Is.InstanceOf<JsVariableDeclarationStatement>());
            Assert.That(OutputFormatter.Format(stmt.InitStatement).Replace("\r\n", "\n"), Is.EqualTo("var x = 0, y = 0;\n"));
            Assert.That(OutputFormatter.Format(stmt.ConditionExpression), Is.EqualTo("x < y"));
            Assert.That(OutputFormatter.Format(stmt.IteratorExpression), Is.EqualTo("x++"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));

            stmt = ParseStatement<JsForStatement>("for (; x < y; x++) { z; }");
            Assert.That(stmt.InitStatement, Is.InstanceOf<JsEmptyStatement>());
            Assert.That(OutputFormatter.Format(stmt.ConditionExpression), Is.EqualTo("x < y"));
            Assert.That(OutputFormatter.Format(stmt.IteratorExpression), Is.EqualTo("x++"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));

            stmt = ParseStatement<JsForStatement>("for (x = 0; ; x++) { z; }");
            Assert.That(stmt.InitStatement, Is.InstanceOf<JsExpressionStatement>());
            Assert.That(OutputFormatter.Format(stmt.InitStatement).Replace("\r\n", "\n"), Is.EqualTo("x = 0;\n"));
            Assert.That(stmt.ConditionExpression, Is.Null);
            Assert.That(OutputFormatter.Format(stmt.IteratorExpression), Is.EqualTo("x++"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));

            stmt = ParseStatement<JsForStatement>("for (x = 0; x < y; ) { z; }");
            Assert.That(stmt.InitStatement, Is.InstanceOf<JsExpressionStatement>());
            Assert.That(OutputFormatter.Format(stmt.InitStatement).Replace("\r\n", "\n"), Is.EqualTo("x = 0;\n"));
            Assert.That(OutputFormatter.Format(stmt.ConditionExpression), Is.EqualTo("x < y"));
            Assert.That(stmt.IteratorExpression, Is.Null);
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));
        }

        [Test]
        public void ForEachInStatement()
        {
            var stmt = ParseStatement<JsForEachInStatement>("for (x in y) { z; }");
            Assert.That(stmt.LoopVariableName, Is.EqualTo("x"));
            Assert.That(stmt.IsLoopVariableDeclared, Is.False);
            Assert.That(OutputFormatter.Format(stmt.ObjectToIterateOver), Is.EqualTo("y"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));

            stmt = ParseStatement<JsForEachInStatement>("for (var x in y) { z; }");
            Assert.That(stmt.LoopVariableName, Is.EqualTo("x"));
            Assert.That(stmt.IsLoopVariableDeclared, Is.True);
            Assert.That(OutputFormatter.Format(stmt.ObjectToIterateOver), Is.EqualTo("y"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));
        }

        [Test]
        public void ContinueStatement()
        {
            var stmt = ParseStatement<JsContinueStatement>("continue;");
            Assert.That(stmt.TargetLabel, Is.Null);

            stmt = ParseStatement<JsContinueStatement>("continue lbl;");
            Assert.That(stmt.TargetLabel, Is.EqualTo("lbl"));
        }

        [Test]
        public void BreakStatement()
        {
            var stmt = ParseStatement<JsBreakStatement>("break;");
            Assert.That(stmt.TargetLabel, Is.Null);

            stmt = ParseStatement<JsBreakStatement>("break lbl;");
            Assert.That(stmt.TargetLabel, Is.EqualTo("lbl"));
        }

        [Test]
        public void ReturnStatement()
        {
            var stmt = ParseStatement<JsReturnStatement>("return;");
            Assert.That(stmt.Value, Is.Null);

            stmt = ParseStatement<JsReturnStatement>("return x;");
            Assert.That(OutputFormatter.Format(stmt.Value), Is.EqualTo("x"));
        }

        [Test]
        public void WithStatement()
        {
            var stmt = ParseStatement<JsWithStatement>("with (x) { y; }");
            Assert.That(OutputFormatter.Format(stmt.Object), Is.EqualTo("x"));
            Assert.That(OutputFormatter.Format(stmt.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\ty;\n}\n"));
        }

        [Test]
        public void TryStatement()
        {
            var stmt = ParseStatement<JsTryStatement>("try { a; } catch (b) { c; } finally { d; }");
            Assert.That(OutputFormatter.Format(stmt.GuardedStatement).Replace("\r\n", "\n"), Is.EqualTo("{\n\ta;\n}\n"));
            Assert.That(stmt.Catch.Identifier, Is.EqualTo("b"));
            Assert.That(OutputFormatter.Format(stmt.Catch.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tc;\n}\n"));
            Assert.That(OutputFormatter.Format(stmt.Finally).Replace("\r\n", "\n"), Is.EqualTo("{\n\td;\n}\n"));

            stmt = ParseStatement<JsTryStatement>("try { a; } catch (b) { c; }");
            Assert.That(OutputFormatter.Format(stmt.GuardedStatement).Replace("\r\n", "\n"), Is.EqualTo("{\n\ta;\n}\n"));
            Assert.That(stmt.Catch.Identifier, Is.EqualTo("b"));
            Assert.That(OutputFormatter.Format(stmt.Catch.Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tc;\n}\n"));
            Assert.That(stmt.Finally, Is.Null);

            stmt = ParseStatement<JsTryStatement>("try { a; } finally { d; }");
            Assert.That(OutputFormatter.Format(stmt.GuardedStatement).Replace("\r\n", "\n"), Is.EqualTo("{\n\ta;\n}\n"));
            Assert.That(stmt.Catch, Is.Null);
            Assert.That(OutputFormatter.Format(stmt.Finally).Replace("\r\n", "\n"), Is.EqualTo("{\n\td;\n}\n"));
        }

        [Test]
        public void ThrowStatement()
        {
            var stmt = ParseStatement<JsThrowStatement>("throw x;");
            Assert.That(OutputFormatter.Format(stmt.Expression), Is.EqualTo("x"));
        }

        [Test]
        public void FunctionDeclarationStatement()
        {
            RoundtripStatement("function f(){\n\tx;\n}\n");
            RoundtripStatement("function f(a, b, c){\n\tx;\n}\n");
        }

        [Test]
        public void SwitchStatement()
        {
            var stmt = ParseStatement<JsSwitchStatement>("switch(a) { case b: x; case c: y; default: z; }");
            Assert.That(OutputFormatter.Format(stmt.Expression), Is.EqualTo("a"));
            Assert.That(stmt.Sections.Count, Is.EqualTo(3));
            Assert.That(stmt.Sections[0].Values.Select(v => OutputFormatter.Format(v)), Is.EqualTo(new[] { "b" }));
            Assert.That(OutputFormatter.Format(stmt.Sections[0].Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tx;\n}\n"));
            Assert.That(stmt.Sections[1].Values.Select(v => OutputFormatter.Format(v)), Is.EqualTo(new[] { "c" }));
            Assert.That(OutputFormatter.Format(stmt.Sections[1].Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\ty;\n}\n"));
            Assert.That(stmt.Sections[2].Values, Is.EqualTo(new object[] { null }));
            Assert.That(OutputFormatter.Format(stmt.Sections[2].Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tz;\n}\n"));
        }

        [Test]
        public void SwitchStatementWithMultipleLabelsPerBlock()
        {
            var stmt = ParseStatement<JsSwitchStatement>("switch(a) { case b: case c: x; case d: default: y; }");
            Assert.That(OutputFormatter.Format(stmt.Expression), Is.EqualTo("a"));
            Assert.That(stmt.Sections.Count, Is.EqualTo(2));
            Assert.That(stmt.Sections[0].Values.Select(v => OutputFormatter.Format(v)), Is.EqualTo(new[] { "b", "c" }));
            Assert.That(OutputFormatter.Format(stmt.Sections[0].Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\tx;\n}\n"));
            Assert.That(stmt.Sections[1].Values.Count, Is.EqualTo(2));
            Assert.That(OutputFormatter.Format(stmt.Sections[1].Values[0]), Is.EqualTo("d"));
            Assert.That(stmt.Sections[1].Values[1], Is.Null);
            Assert.That(OutputFormatter.Format(stmt.Sections[1].Body).Replace("\r\n", "\n"), Is.EqualTo("{\n\ty;\n}\n"));
        }

        [Test]
        public void EmptySwitchStatement()
        {
            var stmt = ParseStatement<JsSwitchStatement>("switch(a) {}");
            Assert.That(OutputFormatter.Format(stmt.Expression), Is.EqualTo("a"));
            Assert.That(stmt.Sections.Count, Is.EqualTo(0));
        }

        [Test]
        public void SwitchStatementWithEmptyClause()
        {
            var stmt = ParseStatement<JsSwitchStatement>("switch(a) { case b: }");
            Assert.That(OutputFormatter.Format(stmt.Expression), Is.EqualTo("a"));
            Assert.That(stmt.Sections.Count, Is.EqualTo(1));
            Assert.That(stmt.Sections[0].Values.Select(v => OutputFormatter.Format(v)), Is.EqualTo(new[] { "b" }));
            Assert.That(stmt.Sections[0].Body.Statements.Count, Is.EqualTo(0));
        }
    }
}
