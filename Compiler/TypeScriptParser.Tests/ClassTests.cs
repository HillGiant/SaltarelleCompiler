using NUnit.Framework;
using System.Linq;
using TypeScriptModel.TypeSystem.Elements;

namespace TypeScriptParser.Tests
{
    using TypeScriptModel.Elements.ClassMembers;
    using TypeScriptModel.Statements;
    using TypeScriptModel.TypeSystem;

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

        [Test]
        public void ClassWithEmptyConstructor()
        {
            var input =
@"class foo {
    constructor(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var constructor = (TsConstructorDeclaration)type.Members.First();
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            Assert.That(constructor.Signatures.First().Accessibility, Is.Null);
            Assert.That(constructor.Signatures.First().Parameters, Is.Null);
            Assert.That(constructor.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithPublicConstructor()
        {
            var input =
@"class foo {
    public constructor(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var constructor = (TsConstructorDeclaration)type.Members.First();
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            Assert.That(constructor.Signatures.First().Accessibility, Is.EqualTo(AccessibilityModifier.Public));
            Assert.That(constructor.Signatures.First().Parameters, Is.Null);
            Assert.That(constructor.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithPrivateConstructor()
        {
            var input =
@"class foo {
    private constructor(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var constructor = (TsConstructorDeclaration)type.Members.First();
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            Assert.That(constructor.Signatures.First().Accessibility, Is.EqualTo(AccessibilityModifier.Private));
            Assert.That(constructor.Signatures.First().Parameters, Is.Null);
            Assert.That(constructor.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithProtectedConstructor()
        {
            var input =
@"class foo {
    protected constructor(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var constructor = (TsConstructorDeclaration)type.Members.First();
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            Assert.That(constructor.Signatures.First().Accessibility, Is.EqualTo(AccessibilityModifier.Protected));
            Assert.That(constructor.Signatures.First().Parameters, Is.Null);
            Assert.That(constructor.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructorWithParameterNoTypeAnnotation()
        {
            var input =
@"class foo {
    public constructor(foo){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructorDeclaration>());

            var constructor = type.Members[0] as TsConstructorDeclaration;
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            var constSig = constructor.Signatures.First();
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructorSignatureWithParameter()
        {
            var input =
@"class foo {
    public constructor(foo: string){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructorDeclaration>());

            var constructor = type.Members[0] as TsConstructorDeclaration;
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            var constSig = constructor.Signatures.First();
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = constSig.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(constSig.Parameters[0].Optional, Is.False);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructorWithOptionalParameter()
        {
            var input =
@"class foo {
    public constructor(foo?: string){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructorDeclaration>());

            var constructor = type.Members[0] as TsConstructorDeclaration;
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            var constSig = constructor.Signatures.First();
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(constSig.Parameters[0].Optional, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructorWithRestParameter()
        {
            var input =
@"class foo {
    public constructor(foo?: string, ...bar){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructorDeclaration>());

            var constructor = type.Members[0] as TsConstructorDeclaration;
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            var constSig = constructor.Signatures.First();
            Assert.That(constSig.Parameters.Count, Is.EqualTo(2));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(constSig.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(constSig.Parameters[1].Type, Is.Null);
            Assert.That(constSig.Parameters[1].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructorWithOnlyRestParameter()
        {
            var input =
@"class foo {
    public constructor(...bar){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructorDeclaration>());

            var constructor = type.Members[0] as TsConstructorDeclaration;
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            var constSig = constructor.Signatures.First();
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);
            Assert.That(constSig.Parameters[0].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructorWithBody()
        {
            var input =
@"class foo {
    public constructor(...bar){
        for (x = 0; x < y; x++) {
            z;
        }
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructorDeclaration>());

            var constructor = type.Members[0] as TsConstructorDeclaration;
            Assert.That(constructor.Signatures.Count, Is.EqualTo(1));
            var constSig = constructor.Signatures.First();
            Assert.That(constSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(constSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(constSig.Parameters[0].Type, Is.Null);
            Assert.That(constSig.Parameters[0].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ConstructorWithOverload()
        {
            var input =
@"class foo {
    public constructor();
    public constructor(bar){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsConstructorDeclaration>());

            var constructor = type.Members[0] as TsConstructorDeclaration;
            Assert.That(constructor.Signatures.Count, Is.EqualTo(2));
            var firstSig = constructor.Signatures.First();
            Assert.That(firstSig.Parameters, Is.Null);

            var secondSig = constructor.Signatures.Last();
            Assert.That(secondSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(secondSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(secondSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithEmptyMethod()
        {
            var input =
@"class foo {
    method(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var method = (TsMethodDeclaration)type.Members.First();
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            Assert.That(method.Signatures.First().Accessibility, Is.Null);
            Assert.That(method.Signatures.First().Parameters, Is.Null);
            Assert.That(method.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithPublicMethod()
        {
            var input =
@"class foo {
    public method(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var method = (TsMethodDeclaration)type.Members.First();
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            Assert.That(method.Signatures.First().Accessibility, Is.EqualTo(AccessibilityModifier.Public));
            Assert.That(method.Signatures.First().Parameters, Is.Null);
            Assert.That(method.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithPrivateMethod()
        {
            var input =
@"class foo {
    private method(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var method = (TsMethodDeclaration)type.Members.First();
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            Assert.That(method.Signatures.First().Accessibility, Is.EqualTo(AccessibilityModifier.Private));
            Assert.That(method.Signatures.First().Parameters, Is.Null);
            Assert.That(method.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithProtectedMethod()
        {
            var input =
@"class foo {
    protected method(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var method = (TsMethodDeclaration)type.Members.First();
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            Assert.That(method.Signatures.First().Accessibility, Is.EqualTo(AccessibilityModifier.Protected));
            Assert.That(method.Signatures.First().Parameters, Is.Null);
            Assert.That(method.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodWithParameterNoTypeAnnotation()
        {
            var input =
@"class foo {
    public method(foo){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodDeclaration>());

            var method = type.Members[0] as TsMethodDeclaration;
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            var methSig = method.Signatures.First();
            Assert.That(methSig.Name, Is.EqualTo("method"));
            Assert.That(methSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodSignatureWithParameter()
        {
            var input =
@"class foo {
    public method(foo: string){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodDeclaration>());

            var method = type.Members[0] as TsMethodDeclaration;
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            var methSig = method.Signatures.First();
            Assert.That(methSig.Name, Is.EqualTo("method"));
            Assert.That(methSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = methSig.Parameters[0].Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(methSig.Parameters[0].Optional, Is.False);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodWithOptionalParameter()
        {
            var input =
@"class foo {
    public method(foo?: string){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodDeclaration>());

            var method = type.Members[0] as TsMethodDeclaration;
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            var methSig = method.Signatures.First();
            Assert.That(methSig.Name, Is.EqualTo("method"));
            Assert.That(methSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methSig.Parameters[0].Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(methSig.Parameters[0].Optional, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodWithRestParameter()
        {
            var input =
@"class foo {
    public method(foo?: string, ...bar){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodDeclaration>());

            var method = type.Members[0] as TsMethodDeclaration;
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            var methSig = method.Signatures.First();
            Assert.That(methSig.Name, Is.EqualTo("method"));
            Assert.That(methSig.Parameters.Count, Is.EqualTo(2));
            Assert.That(methSig.Parameters[0].Name, Is.EqualTo("foo"));
            Assert.That(methSig.Parameters[1].Name, Is.EqualTo("bar"));
            Assert.That(methSig.Parameters[1].Type, Is.Null);
            Assert.That(methSig.Parameters[1].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodWithOnlyRestParameter()
        {
            var input =
@"class foo {
    public method(...bar){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodDeclaration>());

            var method = type.Members[0] as TsMethodDeclaration;
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            var methSig = method.Signatures.First();
            Assert.That(methSig.Name, Is.EqualTo("method"));
            Assert.That(methSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(methSig.Parameters[0].Type, Is.Null);
            Assert.That(methSig.Parameters[0].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodWithBody()
        {
            var input =
@"class foo {
    public method(...bar){
        for (x = 0; x < y; x++) {
            z;
        }
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodDeclaration>());

            var method = type.Members[0] as TsMethodDeclaration;
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            var methSig = method.Signatures.First();
            Assert.That(methSig.Name, Is.EqualTo("method"));
            Assert.That(methSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(methSig.Parameters[0].Type, Is.Null);
            Assert.That(methSig.Parameters[0].ParamArray, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void MethodWithOverload()
        {
            var input =
@"class foo {
    public method();
    public method(bar){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodDeclaration>());

            var method = type.Members[0] as TsMethodDeclaration;
            Assert.That(method.Signatures.Count, Is.EqualTo(2));
            var firstSig = method.Signatures.First();
            Assert.That(firstSig.Parameters, Is.Null);

            var secondSig = method.Signatures.Last();
            Assert.That(secondSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(secondSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(secondSig.Parameters[0].Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void StaticMethod()
        {
            var input =
@"class foo {
    public static method(bar){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsMethodDeclaration>());

            var method = type.Members[0] as TsMethodDeclaration;
            Assert.That(method.Signatures.Count, Is.EqualTo(1));
            var methSig = method.Signatures.First();
            Assert.That(methSig.Parameters.Count, Is.EqualTo(1));
            Assert.That(methSig.Parameters[0].Name, Is.EqualTo("bar"));
            Assert.That(methSig.Parameters[0].Type, Is.Null);
            Assert.That(methSig.IsStatic);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void IndexSignatureStringParameter()
        {
            var input =
@"class q {
    [foo: string]: typeRef;
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassIndexSignature>());

            var indexSig = (type.Members[0] as TsClassIndexSignature).Signature;
            Assert.That(indexSig.ParameterName, Is.EqualTo("foo"));
            Assert.That(indexSig.ParameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(indexSig.ReturnType, Is.TypeOf<TsTypeReference>());

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void IndexSignatureNumberParameter()
        {
            var input =
@"class q {
    [foo: number]: typeRef;
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassIndexSignature>());

            var indexSig = (type.Members[0] as TsClassIndexSignature).Signature;
            Assert.That(indexSig.ParameterName, Is.EqualTo("foo"));
            Assert.That(indexSig.ParameterType.Primitive, Is.EqualTo(TsPrimitive.Number));
            Assert.That(indexSig.ReturnType, Is.TypeOf<TsTypeReference>());

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithEmptyGetter()
        {
            var input =
@"class foo {
    get bar(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var getter = (TsClassGetAccessor)type.Members.First();
            Assert.That(getter.Accessibility, Is.Null);
            Assert.That(getter.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithPublicGetter()
        {
            var input =
@"class foo {
    public get bar(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var getter = (TsClassGetAccessor)type.Members.First();
            Assert.That(getter.Accessibility, Is.EqualTo(AccessibilityModifier.Public));
            Assert.That(getter.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithPrivateGetter()
        {
            var input =
@"class foo {
    private get bar(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var getter = (TsClassGetAccessor)type.Members.First();
            Assert.That(getter.Accessibility, Is.EqualTo(AccessibilityModifier.Private));
            Assert.That(getter.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithProtectedGetter()
        {
            var input =
@"class foo {
    protected get bar(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var getter = (TsClassGetAccessor)type.Members.First();
            Assert.That(getter.Accessibility, Is.EqualTo(AccessibilityModifier.Protected));
            Assert.That(getter.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void GetterWithBody()
        {
            var input =
@"class foo {
    public get bar(){
        for (x = 0; x < y; x++) {
            z;
        }
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassGetAccessor>());

            var getter = type.Members[0] as TsClassGetAccessor;
            Assert.That(getter.Name, Is.EqualTo("bar"));

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void StaticGetter()
        {
            var input =
@"class foo {
    public static get baz(){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassGetAccessor>());

            var getter = type.Members[0] as TsClassGetAccessor;
            Assert.That(getter.Accessibility, Is.EqualTo(AccessibilityModifier.Public));
            Assert.That(getter.Name, Is.EqualTo("baz"));
            Assert.That(getter.IsStatic);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void GetterWithReturnType()
        {
            var input =
@"class foo {
    public get bar(): Bork{
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var getter = (TsClassGetAccessor)type.Members.First();
            Assert.That(getter.Accessibility, Is.EqualTo(AccessibilityModifier.Public));
            Assert.That(getter.Body, Is.TypeOf<JsBlockStatement>());
            Assert.That(getter.TypeAnnotation, Is.TypeOf<TsTypeReference>());
            Assert.That((getter.TypeAnnotation as TsTypeReference).Name, Is.EqualTo("Bork"));
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithPrivateSetter()
        {
            var input =
@"class foo {
    private set bar(foo){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var setter = (TsClassSetAccessor)type.Members.First();
            Assert.That(setter.Accessibility, Is.EqualTo(AccessibilityModifier.Private));
            Assert.That(setter.Parameter.Name, Is.EqualTo("foo"));
            Assert.That(setter.Parameter.Type, Is.Null);
            Assert.That(setter.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void ClassWithProtectedSetter()
        {
            var input =
@"class foo {
    protected set bar(foo){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Name, Is.EqualTo("foo"));
            Assert.That(type.Members.Count, Is.EqualTo(1));

            var setter = (TsClassSetAccessor)type.Members.First();
            Assert.That(setter.Accessibility, Is.EqualTo(AccessibilityModifier.Protected));
            Assert.That(setter.Parameter.Name, Is.EqualTo("foo"));
            Assert.That(setter.Parameter.Type, Is.Null);
            Assert.That(setter.Body, Is.TypeOf<JsBlockStatement>());
            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void SetterWithParameterNoTypeAnnotation()
        {
            var input =
@"class foo {
    public set bar(foo){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassSetAccessor>());

            var setter = type.Members[0] as TsClassSetAccessor;
            Assert.That(setter.Name, Is.EqualTo("bar"));
            Assert.That(setter.Parameter.Name, Is.EqualTo("foo"));
            Assert.That(setter.Parameter.Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void SetterSignatureWithParameter()
        {
            var input =
@"class foo {
    public set bar(foo: string){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassSetAccessor>());

            var setter = type.Members[0] as TsClassSetAccessor;
            Assert.That(setter.Name, Is.EqualTo("bar"));
            Assert.That(setter.Parameter.Name, Is.EqualTo("foo"));
            Assert.That(setter.Parameter.Type, Is.TypeOf<TsPrimitiveType>());
            var parameterType = setter.Parameter.Type as TsPrimitiveType;
            Assert.That(parameterType.Primitive, Is.EqualTo(TsPrimitive.String));
            Assert.That(setter.Parameter.Optional, Is.False);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void SetterWithOptionalParameter()
        {
            var input =
@"class foo {
    public set bar(foo?: string){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassSetAccessor>());

            var setter = type.Members[0] as TsClassSetAccessor;
            Assert.That(setter.Name, Is.EqualTo("bar"));
            Assert.That(setter.Parameter.Name, Is.EqualTo("foo"));
            Assert.That(setter.Parameter.Type, Is.TypeOf<TsPrimitiveType>());
            Assert.That(setter.Parameter.Optional, Is.True);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void SetterWithBody()
        {
            var input =
@"class foo {
    public set bar(bar){
        for (x = 0; x < y; x++) {
            z;
        }
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassSetAccessor>());

            var setter = type.Members[0] as TsClassSetAccessor;
            Assert.That(setter.Name, Is.EqualTo("bar"));
            Assert.That(setter.Parameter.Name, Is.EqualTo("bar"));
            Assert.That(setter.Parameter.Type, Is.Null);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }

        [Test]
        public void StaticSetter()
        {
            var input =
@"class foo {
    public static set baz(bar){
    }
}
";
            var type = TestUtils.ParseElement<TsClass>(input);
            Assert.That(type.Members.Count, Is.EqualTo(1));
            Assert.That(type.Members[0], Is.TypeOf<TsClassSetAccessor>());

            var setter = type.Members[0] as TsClassSetAccessor;
            Assert.That(setter.Accessibility, Is.EqualTo(AccessibilityModifier.Public));
            Assert.That(setter.Name, Is.EqualTo("baz"));
            Assert.That(setter.Parameter.Name, Is.EqualTo("bar"));
            Assert.That(setter.Parameter.Type, Is.Null);
            Assert.That(setter.IsStatic);

            TestUtils.SerializedTypeMatchesInput(input, type);
        }
    }
}
