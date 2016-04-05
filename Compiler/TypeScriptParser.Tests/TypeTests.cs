namespace TypeScriptParser.Tests
{
    using System;

    using NUnit.Framework;

    using TypeScriptModel;
    using TypeScriptModel.TypeSystem;

    [TestFixture]
    public class TypeTests
    {

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
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ObjectWithCallSignature()
        {
            var input =
@"{
    ();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void CallSignatureWithParameterNoTypeArg()
        {
            var input =
@"{
    (foo);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.ReturnType, Is.Null);
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void CallSignatureWithReturnType()
        {
            var input =
@"{
    (foo): bar;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void CallSignatureWithParameter()
        {
            var input =
@"{
    (foo: string);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = callSig.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(callSig.Parameters[0].Optional, Is.False);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void CallSignatureWithOptionalParameter()
        {
            var input =
@"{
    (foo?: string);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(callSig.Parameters[0].Optional, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void CallSignatureWithRestParameter()
        {
            var input =
@"{
    (foo?: string, ...bar);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.Parameters.Count, Is.EqualTo(2));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(callSig.Parameters[1].Type, Is.Null);
            Assert.That(callSig.Parameters[1].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void CallSignatureWithOnlyRestParameter()
        {
            var input =
@"{
    (...bar);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(callSig.Parameters[0].Type, Is.Null);
            Assert.That(callSig.Parameters[0].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void CallSignatureWithTypeParams()
        {
            var input =
@"{
    <T, U>();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(callSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(callSig.TypeParameters[0].Constraint, Is.Null);
            Assert.That(callSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(callSig.TypeParameters[1].Constraint, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void CallSignatureWithTypeParamsWithExtends()
        {
            var input =
@"{
    <T extends { a: string; b: number; }, U extends foo>();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(callSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(callSig.TypeParameters[0].Constraint, Is.TypeOf<TsObjectType>());
            Assert.That(callSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(callSig.TypeParameters[1].Constraint, Is.TypeOf<TsTypeReference>());

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void IndexSignatureStringParameter()
        {
            var input =
@"{
    [foo: string]: typeRef;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsIndexSignature>());

            var indexSig = type.Members[0] as TsIndexSignature;
            Assert.That(indexSig.ParameterName, Is.EqualTo("foo"));
            Assert.That(indexSig.ParameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(indexSig.ReturnType, Is.TypeOf<TsTypeReference>());

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void IndexSignatureNumberParameter()
        {
            var input =
@"{
    [foo: number]: typeRef;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsIndexSignature>());

            var indexSig = type.Members[0] as TsIndexSignature;
            Assert.That(indexSig.ParameterName, Is.EqualTo("foo"));
            Assert.That(indexSig.ParameterType.Primitive, Is.EqualTo(TsPrimitive.Number));
            Assert.That(indexSig.ReturnType, Is.TypeOf<TsTypeReference>());

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ObjectWithMethodSignature()
        {
            var input =
@"{
    bork();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void MethodSignatureWithParameterNoTypeArg()
        {
            var input =
@"{
    bork(foo);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.ReturnType, Is.Null);
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }



        [Test]
        public void MethodSignatureWithReturnType()
        {
            var input =
@"{
    bork(foo): typeRef;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void OptionalMethodSignature()
        {
            var input =
@"{
    bork?(foo): typeRef;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.True);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void MethodSignatureWithParameter()
        {
            var input =
@"{
    bork(foo: string);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = methodSig.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(methodSig.Parameters[0].Optional, Is.False);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void MethodSignatureWithOptionalParameter()
        {
            var input =
@"{
    bork(foo?: string);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(methodSig.Parameters[0].Optional, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void MethodSignatureWithRestParameter()
        {
            var input =
@"{
    bork(foo?: string, ...bar);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(2));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(methodSig.Parameters[1].Type, Is.Null);
            Assert.That(methodSig.Parameters[1].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void MethodSignatureWithOnlyRestParameter()
        {
            var input =
@"{
    bork(...bar);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(methodSig.Parameters[0].Type, Is.Null);
            Assert.That(methodSig.Parameters[0].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void MethodSignatureWithTypeParams()
        {
            var input =
@"{
    bork<T, U>();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(methodSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(methodSig.TypeParameters[0].Constraint, Is.Null);
            Assert.That(methodSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(methodSig.TypeParameters[1].Constraint, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void MethodSignatureWithTypeParamsWithExtends()
        {
            var input =
@"{
    bork<T extends { a: string; b: number; }, U extends foo>();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(methodSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(methodSig.TypeParameters[0].Constraint, Is.TypeOf<TsObjectType>());
            Assert.That(methodSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(methodSig.TypeParameters[1].Constraint, Is.TypeOf<TsTypeReference>());

            Assert.That(SerializedTypeMatchesInput(input, type));
        }


        [Test]
        public void ObjectWithConstructSignature()
        {
            var input =
@"{
    new ();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructSignatureWithParameterNoTypeArg()
        {
            var input =
@"{
    new (foo);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.ReturnType, Is.Null);
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructSignatureWithReturnType()
        {
            var input =
@"{
    new (foo): bar;
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructSignatureWithParameter()
        {
            var input =
@"{
    new (foo: string);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = constSig.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(constSig.Parameters[0].Optional, Is.False);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructSignatureWithOptionalParameter()
        {
            var input =
@"{
    new (foo?: string);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(constSig.Parameters[0].Optional, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructSignatureWithRestParameter()
        {
            var input =
@"{
    new (foo?: string, ...bar);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.Parameters.Count, Is.EqualTo(2));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(constSig.Parameters[1].Type, Is.Null);
            Assert.That(constSig.Parameters[1].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructSignatureWithOnlyRestParameter()
        {
            var input =
@"{
    new (...bar);
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);
            Assert.That(constSig.Parameters[0].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructSignatureWithTypeParams()
        {
            var input =
@"{
    new <T, U>();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(constSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(constSig.TypeParameters[0].Constraint, Is.Null);
            Assert.That(constSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(constSig.TypeParameters[1].Constraint, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructSignatureWithTypeParamsWithExtends()
        {
            var input =
@"{
    new <T extends { a: string; b: number; }, U extends foo>();
}";
            var type = ParseType<TsObjectType>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(constSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(constSig.TypeParameters[0].Constraint, Is.TypeOf<TsObjectType>());
            Assert.That(constSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(constSig.TypeParameters[1].Constraint, Is.TypeOf<TsTypeReference>());

            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void EmptyObjectArray()
        {
            var input =
@"{
}[]";
            var array = ParseType<TsArrayType>(input);
            Assert.That(array.ElementType, Is.TypeOf<TsObjectType>());
        }

        [Test]
        public void TypeRefArray()
        {
            var input = @"foo[]";
            var array = ParseType<TsArrayType>(input);
            Assert.That(array.ElementType, Is.TypeOf<TsTypeReference>());
        }

        [Test]
        public void TypeRefDoubleArray()
        {
            var input = @"foo[][]";
            var array = ParseType<TsArrayType>(input);
            Assert.That(array.ElementType, Is.TypeOf<TsArrayType>());
            Assert.That(((TsArrayType)array.ElementType).ElementType, Is.TypeOf<TsTypeReference>());
        }

        [Test]
        public void FunctionType()
        {
            var input = @"() => foo";
            var type = ParseType<TsFunctionType>(input);
            Assert.That(type.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void FunctionTypeWithParameterNoTypeArg()
        {
            var input = @"(foo) => bar";
            var funcType = ParseType<TsFunctionType>(input);
            Assert.That(funcType.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(funcType.Parameters.Count, Is.EqualTo(1));
            Assert.That(funcType.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(funcType.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, funcType));
        }

        [Test]
        public void FunctionTypeWithParameter()
        {
            var input = @"(foo: string) => bar";
            var funcType = ParseType<TsFunctionType>(input);
            Assert.That(funcType.Parameters.Count, Is.EqualTo(1));
            Assert.That(funcType.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(funcType.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = funcType.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(funcType.Parameters[0].Optional, Is.False);

            Assert.That(SerializedTypeMatchesInput(input, funcType));
        }

        [Test]
        public void FunctionTypeWithOptionalParameter()
        {
            var input = @"(foo?: string) => bar";
            var funcType = ParseType<TsFunctionType>(input);
            Assert.That(funcType.Parameters.Count, Is.EqualTo(1));
            Assert.That(funcType.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(funcType.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(funcType.Parameters[0].Optional, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, funcType));
        }

        [Test]
        public void FunctionTypeWithRestParameter()
        {
            var input = @"(foo?: string, ...bar) => any";
            var funcType = ParseType<TsFunctionType>(input);
            Assert.That(funcType.Parameters.Count, Is.EqualTo(2));
            Assert.That(funcType.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(funcType.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(funcType.Parameters[1].Type, Is.Null);
            Assert.That(funcType.Parameters[1].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, funcType));
        }

        [Test]
        public void FunctionTypeWithOnlyRestParameter()
        {
            var input = @"(...bar) => any";
            var funcType = ParseType<TsFunctionType>(input);
            Assert.That(funcType.Parameters.Count, Is.EqualTo(1));
            Assert.That(funcType.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(funcType.Parameters[0].Type, Is.Null);
            Assert.That(funcType.Parameters[0].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, funcType));
        }

        [Test]
        public void FunctionTypeWithTypeParams()
        {
            var input =
@"<T, U>() => any";
            var funcType = ParseType<TsFunctionType>(input);
            Assert.That(funcType.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(funcType.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(funcType.TypeParameters[0].Constraint, Is.Null);
            Assert.That(funcType.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(funcType.TypeParameters[1].Constraint, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, funcType));
        }

        [Test]
        public void FunctionTypeWithTypeParamsWithExtends()
        {
            var input = @"<T extends { a: string; b: number; }, U extends foo>() => any";
            var funcType = ParseType<TsFunctionType>(input);
            Assert.That(funcType.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(funcType.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(funcType.TypeParameters[0].Constraint, Is.TypeOf<TsObjectType>());
            Assert.That(funcType.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(funcType.TypeParameters[1].Constraint, Is.TypeOf<TsTypeReference>());

            Assert.That(SerializedTypeMatchesInput(input, funcType));
        }

        [Test]
        public void ConstructorType()
        {
            var input = @"new () => foo";
            var type = ParseType<TsConstructorType>(input);
            Assert.That(type.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(SerializedTypeMatchesInput(input, type));
        }

        [Test]
        public void ConstructorTypeWithParameterNoTypeArg()
        {
            var input = @"new (foo) => bar";
            var constType = ParseType<TsConstructorType>(input);
            Assert.That(constType.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(constType.Parameters.Count, Is.EqualTo(1));
            Assert.That(constType.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constType.Parameters[0].Type, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, constType));
        }

        [Test]
        public void ConstructorTypeWithParameter()
        {
            var input = @"new (foo: string) => bar";
            var constType = ParseType<TsConstructorType>(input);
            Assert.That(constType.Parameters.Count, Is.EqualTo(1));
            Assert.That(constType.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constType.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = constType.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(constType.Parameters[0].Optional, Is.False);

            Assert.That(SerializedTypeMatchesInput(input, constType));
        }

        [Test]
        public void ConstructorTypeWithOptionalParameter()
        {
            var input = @"new (foo?: string) => bar";
            var constType = ParseType<TsConstructorType>(input);
            Assert.That(constType.Parameters.Count, Is.EqualTo(1));
            Assert.That(constType.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constType.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(constType.Parameters[0].Optional, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, constType));
        }

        [Test]
        public void ConstructorTypeWithRestParameter()
        {
            var input = @"new (foo?: string, ...bar) => any";
            var constType = ParseType<TsConstructorType>(input);
            Assert.That(constType.Parameters.Count, Is.EqualTo(2));
            Assert.That(constType.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constType.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(constType.Parameters[1].Type, Is.Null);
            Assert.That(constType.Parameters[1].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, constType));
        }

        [Test]
        public void ConstructorTypeWithOnlyRestParameter()
        {
            var input = @"new (...bar) => any";
            var constType = ParseType<TsConstructorType>(input);
            Assert.That(constType.Parameters.Count, Is.EqualTo(1));
            Assert.That(constType.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(constType.Parameters[0].Type, Is.Null);
            Assert.That(constType.Parameters[0].ParamArray, Is.True);

            Assert.That(SerializedTypeMatchesInput(input, constType));
        }

        [Test]
        public void ConstructorTypeWithTypeParams()
        {
            var input = @"new <T, U>() => any";
            var constType = ParseType<TsConstructorType>(input);
            Assert.That(constType.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(constType.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(constType.TypeParameters[0].Constraint, Is.Null);
            Assert.That(constType.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(constType.TypeParameters[1].Constraint, Is.Null);

            Assert.That(SerializedTypeMatchesInput(input, constType));
        }

        [Test]
        public void ConstructorTypeWithTypeParamsWithExtends()
        {
            var input = @"new <T extends { a: string; b: number; }, U extends foo>() => any";
            var constType = ParseType<TsConstructorType>(input);
            Assert.That(constType.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(constType.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(constType.TypeParameters[0].Constraint, Is.TypeOf<TsObjectType>());
            Assert.That(constType.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(constType.TypeParameters[1].Constraint, Is.TypeOf<TsTypeReference>());

            Assert.That(SerializedTypeMatchesInput(input, constType));
        }

        [Test]
        public void TupleType()
        {
            var input = @"[string]";
            var tupleType = ParseType<TsTupleType>(input);
            Assert.That(tupleType.Types.Count, Is.EqualTo(1));
            Assert.That(tupleType.Types[0], Is.TypeOf<TsPrimitiveType>());
            Assert.That(((TsPrimitiveType)tupleType.Types[0]).Primitive, Is.EqualTo(TsPrimitive.String));

            Assert.That(SerializedTypeMatchesInput(input, tupleType));
        }

        [Test]
        public void TupleTypeMultipleTypes()
        {
            var input = @"[string, number]";
            var tupleType = ParseType<TsTupleType>(input);
            Assert.That(tupleType.Types.Count, Is.EqualTo(2));
            Assert.That(tupleType.Types[0], Is.TypeOf<TsPrimitiveType>());
            Assert.That(((TsPrimitiveType)tupleType.Types[0]).Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(tupleType.Types[1], Is.TypeOf<TsPrimitiveType>());
            Assert.That(((TsPrimitiveType)tupleType.Types[1]).Primitive, Is.EqualTo(TsPrimitive.Number));

            Assert.That(SerializedTypeMatchesInput(input, tupleType));
        }
    }
}
