namespace TypeScriptParser.Tests
{
    using System;

    using NUnit.Framework;
    using Saltarelle.Compiler.JSModel.Expressions;
    using JavaScriptParser;
    using TypeScriptModel;
    
    [TestFixture]
    public class ExpressionTests
    {
        private T ParseExpression<T>(string source) where T : JsExpression
        {
            var expr = Parser.ParseExpression("(" + source + ")");
            Assert.That(expr, Is.InstanceOf<T>());
            return (T)expr;
        }

        private void RoundtripExpression(string source, string expected = null)
        {
            var expr = Parser.ParseExpression(source);
            Assert.That(OutputFormatter.Format(expr).Replace("\r\n", "\n"), Is.EqualTo((expected ?? source).Replace("\r\n", "\n")));
        }

        [Test]
        public void Null()
        {
            var expr = ParseExpression<JsConstantExpression>("null");
            Assert.That(expr.NodeType, Is.EqualTo(ExpressionNodeType.Null));
        }

        [Test]
        public void Identifier()
        {
            var expr = ParseExpression<JsIdentifierExpression>("myIdentifier");
            Assert.That(expr.Name, Is.EqualTo("myIdentifier"));
        }

        [Test]
        public void Number()
        {
            var expr = ParseExpression<JsConstantExpression>("123");
            Assert.That(expr.NumberValue, Is.EqualTo(123));
            expr = ParseExpression<JsConstantExpression>("0xff");
            Assert.That(expr.NumberValue, Is.EqualTo(255));
            expr = ParseExpression<JsConstantExpression>("1.375");
            Assert.That(expr.NumberValue, Is.EqualTo(1.375));
            expr = ParseExpression<JsConstantExpression>("0377");
            Assert.That(expr.NumberValue, Is.EqualTo(255));
        }

        [Test]
        public void String()
        {
            var expr = ParseExpression<JsConstantExpression>("'XYZ'");
            Assert.That(expr.StringValue, Is.EqualTo("XYZ"));
            expr = ParseExpression<JsConstantExpression>("\"XYZ\"");
            Assert.That(expr.StringValue, Is.EqualTo("XYZ"));
            expr = ParseExpression<JsConstantExpression>("\"X\\\"YZ\"");
            Assert.That(expr.StringValue, Is.EqualTo("X\"YZ"));
        }

        [Test]
        public void Boolean()
        {
            var exprT = ParseExpression<JsConstantExpression>("true");
            Assert.That(exprT.BooleanValue, Is.True);
            var exprF = ParseExpression<JsConstantExpression>("false");
            Assert.That(exprF.BooleanValue, Is.False);
        }

        [Test]
        public void Regex()
        {
            var expr = ParseExpression<JsConstantExpression>("/a/");
            Assert.That(expr.RegexpValue.Pattern, Is.EqualTo("a"));
            Assert.That(expr.RegexpValue.Options, Is.EqualTo(""));
            expr = ParseExpression<JsConstantExpression>("/b/i");
            Assert.That(expr.RegexpValue.Pattern, Is.EqualTo("b"));
            Assert.That(expr.RegexpValue.Options, Is.EqualTo("i"));
        }

        [Test]
        public void This()
        {
            ParseExpression<JsThisExpression>("this");
        }

        [Test]
        public void Unary()
        {
            foreach (var t in new[] { Tuple.Create("typeof x", ExpressionNodeType.TypeOf),
			                          Tuple.Create("!x", ExpressionNodeType.LogicalNot),
			                          Tuple.Create("-x", ExpressionNodeType.Negate),
			                          Tuple.Create("+x", ExpressionNodeType.Positive),
			                          Tuple.Create("++x", ExpressionNodeType.PrefixPlusPlus),
			                          Tuple.Create("--x", ExpressionNodeType.PrefixMinusMinus),
			                          Tuple.Create("x++", ExpressionNodeType.PostfixPlusPlus),
			                          Tuple.Create("x--", ExpressionNodeType.PostfixMinusMinus),
			                          Tuple.Create("delete x", ExpressionNodeType.Delete),
			                          Tuple.Create("void x", ExpressionNodeType.Void),
			                          Tuple.Create("~x", ExpressionNodeType.BitwiseNot),
			                        }
            )
            {
                var expr = ParseExpression<JsUnaryExpression>(t.Item1);
                Assert.That(expr.NodeType, Is.EqualTo(t.Item2));
                Assert.That(expr.Operand.NodeType == ExpressionNodeType.Identifier && ((JsIdentifierExpression)expr.Operand).Name == "x");
            }
        }

        [Test]
        public void Binary()
        {
            foreach (var t in new[] { Tuple.Create("x && y", ExpressionNodeType.LogicalAnd),
			                          Tuple.Create("x || y", ExpressionNodeType.LogicalOr),
			                          Tuple.Create("x != y", ExpressionNodeType.NotEqual),
			                          Tuple.Create("x <= y", ExpressionNodeType.LesserOrEqual),
			                          Tuple.Create("x >= y", ExpressionNodeType.GreaterOrEqual),
			                          Tuple.Create("x < y", ExpressionNodeType.Lesser),
			                          Tuple.Create("x > y", ExpressionNodeType.Greater),
			                          Tuple.Create("x == y", ExpressionNodeType.Equal),
			                          Tuple.Create("x - y", ExpressionNodeType.Subtract),
			                          Tuple.Create("x + y", ExpressionNodeType.Add),
			                          Tuple.Create("x % y", ExpressionNodeType.Modulo),
			                          Tuple.Create("x / y", ExpressionNodeType.Divide),
			                          Tuple.Create("x * y", ExpressionNodeType.Multiply),
			                          Tuple.Create("x & y", ExpressionNodeType.BitwiseAnd),
			                          Tuple.Create("x | y", ExpressionNodeType.BitwiseOr),
			                          Tuple.Create("x ^ y", ExpressionNodeType.BitwiseXor),
			                          Tuple.Create("x === y", ExpressionNodeType.Same),
			                          Tuple.Create("x !== y", ExpressionNodeType.NotSame),
			                          Tuple.Create("x << y", ExpressionNodeType.LeftShift),
			                          Tuple.Create("x >> y", ExpressionNodeType.RightShiftSigned),
			                          Tuple.Create("x >>> y", ExpressionNodeType.RightShiftUnsigned),
			                          Tuple.Create("x instanceof y", ExpressionNodeType.InstanceOf),
			                          Tuple.Create("x in y", ExpressionNodeType.In),
			                          Tuple.Create("x = y", ExpressionNodeType.Assign),
			                          Tuple.Create("x *= y", ExpressionNodeType.MultiplyAssign),
			                          Tuple.Create("x /= y", ExpressionNodeType.DivideAssign),
			                          Tuple.Create("x %= y", ExpressionNodeType.ModuloAssign),
			                          Tuple.Create("x += y", ExpressionNodeType.AddAssign),
			                          Tuple.Create("x -= y", ExpressionNodeType.SubtractAssign),
			                          Tuple.Create("x <<= y", ExpressionNodeType.LeftShiftAssign),
			                          Tuple.Create("x >>= y", ExpressionNodeType.RightShiftSignedAssign),
			                          Tuple.Create("x >>>= y", ExpressionNodeType.RightShiftUnsignedAssign),
			                          Tuple.Create("x &= y", ExpressionNodeType.BitwiseAndAssign),
			                          Tuple.Create("x |= y", ExpressionNodeType.BitwiseOrAssign),
			                          Tuple.Create("x ^= y", ExpressionNodeType.BitwiseXorAssign),
			                        }
            )
            {
                var expr = ParseExpression<JsBinaryExpression>(t.Item1);
                Assert.That(expr.NodeType, Is.EqualTo(t.Item2));
                Assert.That(expr.Left.NodeType == ExpressionNodeType.Identifier && ((JsIdentifierExpression)expr.Left).Name == "x");
                Assert.That(expr.Right.NodeType == ExpressionNodeType.Identifier && ((JsIdentifierExpression)expr.Right).Name == "y");
            }
        }

        [Test]
        public void ChainingBinaryOperations()
        {
            RoundtripExpression("a * b / c % d");
            RoundtripExpression("a + b + c");
            RoundtripExpression("a + b - c");
        }

        [Test]
        public void Comma()
        {
            RoundtripExpression("a, b, c");
        }

        [Test]
        public void Conditional()
        {
            RoundtripExpression("a ? b : c", "(a ? b : c)");
        }

        [Test]
        public void Invocation()
        {
            RoundtripExpression("f()");
            RoundtripExpression("f(a, b)");
        }

        [Test]
        public void InvocationWithSuffix()
        {
            RoundtripExpression("f(a)(b)[c].d");
        }

        [Test]
        public void ObjectCreation()
        {
            RoundtripExpression("new f()");
            RoundtripExpression("new f(a, b)");
        }

        [Test]
        public void FunctionDefinitionExpression1()
        {
            RoundtripExpression(
@"(function f(){
})()");
        }

        [Test]
        public void FunctionDefinitionExpression2()
        {
            RoundtripExpression(
@"(function f(a, b){
	c;
})()");
        }

        [Test]
        public void FunctionDefinitionExpression3()
        {
            RoundtripExpression(
@"(function(a, b){
	c;
})()");
        }

        [Test]
        public void MembersAndIndexing()
        {
            RoundtripExpression("a[b].c[d].e");
            RoundtripExpression("(new a()).b");
        }

        [Test]
        public void ArrayLiteral()
        {
            RoundtripExpression("[1, 2, 3]");
            RoundtripExpression("[1, , 3]");
        }

        [Test]
        public void ObjectLiteral()
        {
            RoundtripExpression("{ a: b, 'c': d, 1: e }", "{ a: b, c: d, '1': e }");
            var expr = ParseExpression<JsObjectLiteralExpression>("{}");
            Assert.That(expr.Values, Is.Empty);
        }
    }
}
