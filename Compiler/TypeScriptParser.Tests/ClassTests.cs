using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeScriptModel.Elements;
using TypeScriptModel.TypeSystem.Elements;

namespace TypeScriptParser.Tests
{
    [TestFixture]
    public class ClassTests
    {
        [Test]
        public void EmptyClass()
        {
            var input =
@"class foo {
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithExtends()
        {
            var input =
@"class foo extends bar {
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends.Count, Is.EqualTo(1));
            Assert.That(type.Extends[0].Name, Is.EqualTo("bar"));
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithMultipleExtends()
        {
            var input =
@"class foo extends bar, baz {
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends.Count, Is.EqualTo(2));
            Assert.That(type.Extends[0].Name, Is.EqualTo("bar"));
            Assert.That(type.Extends[1].Name, Is.EqualTo("baz"));
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithTypeParameter()
        {
            var input =
@"class foo<T> {
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters.Count, Is.EqualTo(1));
            Assert.That(type.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithMultipleTypeParameters()
        {
            var input =
@"class foo<T, U> {
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(type.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(type.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }
    }
}
