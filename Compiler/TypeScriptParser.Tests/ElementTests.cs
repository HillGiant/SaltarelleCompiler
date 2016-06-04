namespace TypeScriptParser.Tests
{
    using System.Linq;

    using NUnit.Framework;

    using TypeScriptModel;
    using TypeScriptModel.Elements;
    using TypeScriptModel.TypeSystem;

    [TestFixture]
    public class ElementTests
    {
        [Test]
        public void EmptyInterface()
        {
            var input =
@"interface foo {
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void InterfaceWithExtends()
        {
            var input =
@"interface foo extends bar {
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends.Count, Is.EqualTo(1));
            Assert.That(type.Extends[0].Name, Is.EqualTo("bar"));
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void InterfaceWithMultipleExtends()
        {
            var input =
@"interface foo extends bar, baz {
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends.Count, Is.EqualTo(2));
            Assert.That(type.Extends[0].Name, Is.EqualTo("bar"));
            Assert.That(type.Extends[1].Name, Is.EqualTo("baz"));
            Assert.That(type.TypeParameters, Is.Null);
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void InterfaceWithTypeParameter()
        {
            var input =
@"interface foo<T> {
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters.Count, Is.EqualTo(1));
            Assert.That(type.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void InterfaceWithMultipleTypeParameters()
        {
            var input =
@"interface foo<T, U> {
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Extends, Is.Null);
            Assert.That(type.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(type.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(type.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(type.Members.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ObjectWithProperty()
        {
            var input =
@"interface q {
    foo;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsPropertySignature>());
            var propSig = type.Members[0] as TsPropertySignature;
            Assert.That(propSig.Name, Is.EqualTo("foo"));
            Assert.That(propSig.Optional, Is.False);
            Assert.That(propSig.Type, Is.Null);
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ObjectWithOptionalProperty()
        {
            var input =
@"interface q {
    foo?;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsPropertySignature>());
            var propSig = type.Members[0] as TsPropertySignature;
            Assert.That(propSig.Name, Is.EqualTo("foo"));
            Assert.That(propSig.Optional, Is.True);
            Assert.That(propSig.Type, Is.Null);
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ObjectWithPropertyWithTypeAnnotation()
        {
            var input =
@"interface q {
    foo?: string;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsPropertySignature>());
            var propSig = type.Members[0] as TsPropertySignature;
            Assert.That(propSig.Name, Is.EqualTo("foo"));
            Assert.That(propSig.Optional, Is.True);
            Assert.That(propSig.Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That((propSig.Type as TsPrimitiveType).Primitive, Is.EqualTo(TsPrimitive.String));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ObjectWithMultipleProperties()
        {
            var input =
@"interface q {
    foo?: string;
    bar: number;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(2));
            Assert.That(type.Members[0], Is.TypeOf<TsPropertySignature>());
            Assert.That(type.Members[1], Is.TypeOf<TsPropertySignature>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ObjectWithCallSignature()
        {
            var input =
@"interface q {
    ();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void CallSignatureWithParameterNoTypeArg()
        {
            var input =
@"interface q {
    (foo);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.ReturnType, Is.Null);
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void CallSignatureWithReturnType()
        {
            var input =
@"interface q {
    (foo): bar;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void CallSignatureWithParameter()
        {
            var input =
@"interface q {
    (foo: string);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = callSig.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(callSig.Parameters[0].Optional, Is.False);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void CallSignatureWithOptionalParameter()
        {
            var input =
@"interface q {
    (foo?: string);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(callSig.Parameters[0].Optional, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void CallSignatureWithRestParameter()
        {
            var input =
@"interface q {
    (foo?: string, ...bar);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.Parameters.Count, Is.EqualTo(2));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(callSig.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(callSig.Parameters[1].Type, Is.Null);
            Assert.That(callSig.Parameters[1].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void CallSignatureWithOnlyRestParameter()
        {
            var input =
@"interface q {
    (...bar);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(callSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(callSig.Parameters[0].Type, Is.Null);
            Assert.That(callSig.Parameters[0].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void CallSignatureWithTypeParams()
        {
            var input =
@"interface q {
    <T, U>();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(callSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(callSig.TypeParameters[0].Constraint, Is.Null);
            Assert.That(callSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(callSig.TypeParameters[1].Constraint, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void CallSignatureWithTypeParamsWithExtends()
        {
            var input =
@"interface q {
    <T extends { a: string; b: number; }, U extends foo>();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsCallSignature>());

            var callSig = type.Members[0] as TsCallSignature;
            Assert.That(callSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(callSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(callSig.TypeParameters[0].Constraint, Is.TypeOf<TsObjectType>());
            Assert.That(callSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(callSig.TypeParameters[1].Constraint, Is.TypeOf<TsTypeReference>());

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void IndexSignatureStringParameter()
        {
            var input =
@"interface q {
    [foo: string]: typeRef;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsIndexSignature>());

            var indexSig = type.Members[0] as TsIndexSignature;
            Assert.That(indexSig.ParameterName, Is.EqualTo("foo"));
            Assert.That(indexSig.ParameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(indexSig.ReturnType, Is.TypeOf<TsTypeReference>());

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void IndexSignatureNumberParameter()
        {
            var input =
@"interface q {
    [foo: number]: typeRef;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsIndexSignature>());

            var indexSig = type.Members[0] as TsIndexSignature;
            Assert.That(indexSig.ParameterName, Is.EqualTo("foo"));
            Assert.That(indexSig.ParameterType.Primitive, Is.EqualTo(TsPrimitive.Number));
            Assert.That(indexSig.ReturnType, Is.TypeOf<TsTypeReference>());

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ObjectWithMethodSignature()
        {
            var input =
@"interface q {
    bork();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodSignatureWithParameterNoTypeArg()
        {
            var input =
@"interface q {
    bork(foo);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.ReturnType, Is.Null);
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }



        [Test]
        public void MethodSignatureWithReturnType()
        {
            var input =
@"interface q {
    bork(foo): typeRef;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void OptionalMethodSignature()
        {
            var input =
@"interface q {
    bork?(foo): typeRef;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.True);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodSignatureWithParameter()
        {
            var input =
@"interface q {
    bork(foo: string);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
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

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodSignatureWithOptionalParameter()
        {
            var input =
@"interface q {
    bork(foo?: string);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methodSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(methodSig.Parameters[0].Optional, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodSignatureWithRestParameter()
        {
            var input =
@"interface q {
    bork(foo?: string, ...bar);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
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

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodSignatureWithOnlyRestParameter()
        {
            var input =
@"interface q {
    bork(...bar);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodSignature>());

            var methodSig = type.Members[0] as TsMethodSignature;
            Assert.That(methodSig.Name, Is.EqualTo("bork"));
            Assert.That(methodSig.Optional, Is.False);
            Assert.That(methodSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methodSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(methodSig.Parameters[0].Type, Is.Null);
            Assert.That(methodSig.Parameters[0].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodSignatureWithTypeParams()
        {
            var input =
@"interface q {
    bork<T, U>();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
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

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodSignatureWithTypeParamsWithExtends()
        {
            var input =
@"interface q {
    bork<T extends { a: string; b: number; }, U extends foo>();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
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

            TestUtils.SerializedTypeMatchesInput(input, type);
        }


        [Test]
        public void ObjectWithConstructSignature()
        {
            var input =
@"interface q {
    new ();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructSignatureWithParameterNoTypeArg()
        {
            var input =
@"interface q {
    new (foo);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.ReturnType, Is.Null);
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructSignatureWithReturnType()
        {
            var input =
@"interface q {
    new (foo): bar;
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.ReturnType, Is.TypeOf<TsTypeReference>());
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructSignatureWithParameter()
        {
            var input =
@"interface q {
    new (foo: string);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = constSig.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(constSig.Parameters[0].Optional, Is.False);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructSignatureWithOptionalParameter()
        {
            var input =
@"interface q {
    new (foo?: string);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(constSig.Parameters[0].Optional, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructSignatureWithRestParameter()
        {
            var input =
@"interface q {
    new (foo?: string, ...bar);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.Parameters.Count, Is.EqualTo(2));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(constSig.Parameters[1].Type, Is.Null);
            Assert.That(constSig.Parameters[1].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructSignatureWithOnlyRestParameter()
        {
            var input =
@"interface q {
    new (...bar);
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);
            Assert.That(constSig.Parameters[0].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructSignatureWithTypeParams()
        {
            var input =
@"interface q {
    new <T, U>();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(constSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(constSig.TypeParameters[0].Constraint, Is.Null);
            Assert.That(constSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(constSig.TypeParameters[1].Constraint, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructSignatureWithTypeParamsWithExtends()
        {
            var input =
@"interface q {
    new <T extends { a: string; b: number; }, U extends foo>();
}";
            var type = TestUtils.ParseElement<TsInterface>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructSignature>());

            var constSig = type.Members[0] as TsConstructSignature;
            Assert.That(constSig.TypeParameters.Count, Is.EqualTo(2));
            Assert.That(constSig.TypeParameters[0].Name, Is.EqualTo("T"));
            Assert.That(constSig.TypeParameters[0].Constraint, Is.TypeOf<TsObjectType>());
            Assert.That(constSig.TypeParameters[1].Name, Is.EqualTo("U"));
            Assert.That(constSig.TypeParameters[1].Constraint, Is.TypeOf<TsTypeReference>());

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void EmptyModule()
        {
            var input =
@"declare module ""foo"" {
}";
            var type = TestUtils.ParseElement<TsAmbientDeclaration>(input);
            Assert.That(type.Declared, Is.TypeOf<TsModule>());
            var module = type.Declared as TsModule;
            Assert.That(module.Name, Is.EqualTo("foo"));
            Assert.That(module.Elements.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void EmptyModuleWithQName()
        {
            var input =
@"declare module ""foo.bar"" {
}";
            var type = TestUtils.ParseElement<TsAmbientDeclaration>(input);
            Assert.That(type.Declared, Is.TypeOf<TsModule>());
            var module = type.Declared as TsModule;
            Assert.That(module.Name, Is.EqualTo("foo.bar"));
            Assert.That(module.Elements.Count, Is.EqualTo(0));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ModuleWithClass()
        {
            var input =
@"declare module ""foo"" {
    class bar {
    }

}";
            var type = TestUtils.ParseElement<TsAmbientDeclaration>(input);
            Assert.That(type.Declared, Is.TypeOf<TsModule>());
            var module = type.Declared as TsModule;
            Assert.That(module.Name, Is.EqualTo("foo"));
            Assert.That(module.Elements.Count, Is.EqualTo(1));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ModuleWithExport()
        {
            var input =
@"declare module ""foo"" {
    export interface bar {
    }
}";
            var type = TestUtils.ParseElement<TsAmbientDeclaration>(input);
            Assert.That(type.Declared, Is.TypeOf<TsModule>());
            var module = type.Declared as TsModule;
            Assert.That(module.Name, Is.EqualTo("foo"));
            Assert.That(module.Elements.Count, Is.EqualTo(1));
            Assert.That(module.Elements.First(), Is.TypeOf<TsExportElement>());
            Assert.That((module.Elements.First() as TsExportElement).Exported, Is.TypeOf<TsInterface>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void AmbientDeclaration()
        {
            var input =
@"declare interface bar {
}";
            var type = TestUtils.ParseElement<TsAmbientDeclaration>(input);
            Assert.That(type.Declared, Is.TypeOf<TsInterface>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }
    }
}
