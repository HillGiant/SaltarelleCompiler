namespace TypeScriptParser.Tests
{
    using System;

    using NUnit.Framework;

    using TypeScriptModel;
    using TypeScriptModel.TypeSystem;

    [TestFixture]
    public class TypeTests
    {
        private class ThrowingErrorReporter : IErrorReporter
        {
            public void ReportError(int line, int col, string message)
            {
                throw new Exception(string.Format("Error at {0},{1}: {2}", line, col, message));
            }
        }

        private T ParseType<T>(string source) where T : TsType
        {
            var expr = Parser.ParseType(source, new ThrowingErrorReporter());
            Assert.That(expr, Is.InstanceOf<T>());
            return (T)expr;
        }

        private void RoundtripType(string source, string expected = null)
        {
            var type = Parser.ParseType(source, new ThrowingErrorReporter());
            Assert.That(SerializedTypeMatchesInput(source, type));
        }

        private bool SerializedTypeMatchesInput(string source, TsType type)
        {
            return OutputFormatter.Format(type, false).Replace("\r\n", "\n").Replace("\t", "    ") == source.Replace("\r\n", "\n").Replace("\t", "    ");
        }

        [Test]
        public void TypeReferenceNoTypeArgs()
        {
            var input = "ReferencedType";
            var type = ParseType<TsTypeReference>(input);
            Assert.That(type.Name, Is.EqualTo("ReferencedType"));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void TypeReferenceQName()
        {
            var input = "ReferencedType.foo.bar";
            var type = ParseType<TsTypeReference>(input);
            Assert.That(type.Name, Is.EqualTo("ReferencedType.foo.bar"));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void TypeReferenceWithArgs()
        {
            var input = "ReferencedType<foo, bar, baz>";
            var type = ParseType<TsTypeReference>(input);
            Assert.That(type.Name, Is.EqualTo("ReferencedType"));
            Assert.That(type.TypeArgs.Count, Is.EqualTo(3));
            Assert.That(type.TypeArgs[0], Is.TypeOf<TsTypeReference>());
            Assert.That(type.TypeArgs[1], Is.TypeOf<TsTypeReference>());
            Assert.That(type.TypeArgs[2], Is.TypeOf<TsTypeReference>());

            Assert.That((type.TypeArgs[0] as TsTypeReference).Name, Is.EqualTo("foo"));
            Assert.That((type.TypeArgs[1] as TsTypeReference).Name, Is.EqualTo("bar"));
            Assert.That((type.TypeArgs[2] as TsTypeReference).Name, Is.EqualTo("baz"));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void AnyPrimitiveType()
        {
            var input = "any";
            var type = ParseType<TsPrimitiveType>(input);
            Assert.That(type.Primitive, Is.EqualTo(TsPrimitive.Any));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void StringPrimitiveType()
        {
            var input = "string";
            var type = ParseType<TsPrimitiveType>(input);
            Assert.That(type.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void NumberPrimitiveType()
        {
            var input = "number";
            var type = ParseType<TsPrimitiveType>(input);
            Assert.That(type.Primitive, Is.EqualTo(TsPrimitive.Number));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void BooleanPrimitiveType()
        {
            var input = "boolean";
            var type = ParseType<TsPrimitiveType>(input);
            Assert.That(type.Primitive, Is.EqualTo(TsPrimitive.Boolean));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void VoidPrimitiveType()
        {
            var input = "void";
            var type = ParseType<TsPrimitiveType>(input);
            Assert.That(type.Primitive, Is.EqualTo(TsPrimitive.Void));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void EmptyObject()
        {
            var input = 
@"{
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ObjectWithParameter()
        {
            var input =
@"{
    foo;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsPropertySignature>());
            var propSig = type.Members[0] as TsPropertySignature;
            Assert.That(propSig.Name, Is.EqualTo("foo"));
            Assert.That(propSig.Optional, Is.False);
            Assert.That(propSig.Type, Is.Null);
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ObjectWithOptionalParameter()
        {
            var input =
@"{
    foo?;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsPropertySignature>());
            var propSig = type.Members[0] as TsPropertySignature;
            Assert.That(propSig.Name, Is.EqualTo("foo"));
            Assert.That(propSig.Optional, Is.True);
            Assert.That(propSig.Type, Is.Null);
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ObjectWithParameterWithTypeAnnotation()
        {
            var input =
@"{
    foo?: string;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsPropertySignature>());
            var propSig = type.Members[0] as TsPropertySignature;
            Assert.That(propSig.Name, Is.EqualTo("foo"));
            Assert.That(propSig.Optional, Is.True);
            Assert.That(propSig.Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That((propSig.Type as TsPrimitiveType).Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ObjectWithMultipleParameters()
        {
            var input =
@"{
    foo?: string;
    bar: number;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(2));
            Assert.That(type.Members[0], Is.TypeOf<TsPropertySignature>());
            Assert.That(type.Members[1], Is.TypeOf<TsPropertySignature>());
        }
    }
}
