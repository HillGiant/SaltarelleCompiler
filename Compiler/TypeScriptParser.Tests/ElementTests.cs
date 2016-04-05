namespace TypeScriptParser.Tests
{
    using NUnit.Framework;

    using TypeScriptModel;
    using TypeScriptModel.Elements;
    using TypeScriptModel.TypeSystem;

    [TestFixture]
    public class ElementTests
    {
        private T ParseElement<T>(string source) where T : TsSourceElement
        {
            var expr = Parser.ParseSourceElement(source, new ThrowingErrorReporter());
            Assert.That(expr, Is.InstanceOf<T>());
            return (T)expr;
        }

        private void RoundtripElement(string source, string expected = null)
        {
            var element = Parser.ParseSourceElement(source, new ThrowingErrorReporter());
            SerializedTypeMatchesInput(source, element);
        }

        private void SerializedTypeMatchesInput(string source, TsSourceElement type)
        {
            Assert.That(OutputFormatter.Format(type, false).Replace("\r\n", "\n").Replace("\t", "    "), Is.EqualTo(source.Replace("\r\n", "\n").Replace("\t", "    ")));
        }

        [Test]
        public void EmptyInterface()
        {
            var input =
@"interface foo {
}";
            var type = ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void InterfaceWithExtends()
        {
            var input =
@"interface foo extends bar {
}";
            var type = ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends.Count, Is.EqualTo(1));
            Assert.That(type.Extends[0].Name, Is.EqualTo("bar"));
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void InterfaceWithMultipleExtends()
        {
            var input =
@"interface foo extends bar, baz {
}";
            var type = ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends.Count, Is.EqualTo(2));
            Assert.That(type.Extends[0].Name, Is.EqualTo("bar"));
            Assert.That(type.Extends[1].Name, Is.EqualTo("baz"));
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void InterfaceWithTypeParameter()
        {
            var input =
@"interface foo<T> {
}";
            var type = ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters.Count, Is.EqualTo(1));
            Assert.That(type.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(type.Members.Count, Is.EqualTo(0));
            SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void InterfaceWithMultipleTypeParameters()
        {
            var input =
@"interface foo<T, U> {
}";
            var type = ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(type.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(type.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(type.Members.Count, Is.EqualTo(0));
            SerializedTypeMatchesInput(input, type);
        }
    }
}
