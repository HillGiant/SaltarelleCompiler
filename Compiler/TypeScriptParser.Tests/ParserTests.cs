﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TypeScriptParser.Tests
{
    using System.IO;
    using TypeScriptModel;
    using JavaScriptParser;

    [TestFixture]
    public class ParserTests
    {

        private void Roundtrip(string s)
        {
            var model = Parser.ParseProgram(s);
            var actual = OutputFormatter.Format(model, true);
            Assert.That(actual.Replace("\r\n", "\n").Replace("\t", "    "), Is.EqualTo(s.Replace("\r\n", "\n").Replace("\t", "    ")));
        }

        [Test]
        public void VariableWithoutType() {
                Roundtrip("declare var myVariable;\n");
        }

        [Test]
        public void VariableWithType() {
                Roundtrip("declare var myVariable: SomeType;\n");
        }

        
        [Test]
        public void GlobalFunction() {
                Roundtrip(
@"declare function myFunction(): SomeType;
declare function myFunction();
");
        }

        [Test]
        public void ArrayType() {
            Roundtrip("declare var v: SomeType[];\n");
        }

        [Test]
        public void ArrayOfArray() {
            Roundtrip("declare var v: SomeType[][];\n");
        }

        [Test]
        public void FunctionType() {
                Roundtrip("declare var v: () => resType;\n");
                Roundtrip("declare var v: (name1: paramType1) => resType;\n");
                Roundtrip("declare var v: (name1: paramType1, name2) => resType;\n");
        }

        [Test]
        public void OptionalParams() {
                Roundtrip("declare var v: (name1: paramType1, name2?: paramType2) => resType;\n");
                Roundtrip("declare var v: (name1?: paramType1, name2?: paramType2) => resType;\n");
        }

        [Test]
        public void ParamArray() {
                Roundtrip("declare var v: (...name1) => resType;\n");
                Roundtrip("declare var v: (name1?: paramType1, ...name2: paramType2[]) => resType;\n");
        }

        [Test]
        public void CompositeType() {
                Roundtrip(
@"declare var v: {
    member1: any;
    member2: type2;
};
");
        }

        [Test]
        public void Constructor() {
                Roundtrip(
@"declare var v: {
    new ();
    new (): ReturnType1;
    new (a): ReturnType2;
    new (a?: type1, ...b: type2[]): ReturnType3;
};
");
        }

        [Test]
        public void Indexer() {
                Roundtrip(
@"declare var v: {
    [arg1: string]: ReturnType0;
    [arg2: string]: ReturnType1;
    [arg3: number]: ReturnType2;
};
");
        }

        [Test]
        public void MemberFunctions() {
                Roundtrip(
@"declare var v: {
    member1();
    member2(): returnType1;
    member3(arg1): returnType2;
    member4(arg1?: type1, ...arg2): returnType2;
};
");
        }

        [Test]
        public void NamelessMemberFunction() {
                Roundtrip(
@"declare var v: {
    ();
    (): returnType1;
    (arg1): returnType2;
    (arg1?: type1, ...arg2): returnType2;
};
");
        }

        [Test]
        public void MultipleMemberTypes() {
                Roundtrip(
@"declare var v: {
    (arg1): returnType1;
    method1(arg2): returnType2;
    new (): returnType3;
    [arg1: string]: returnType4;
    var1: returnType5;
};
");
        }

        [Test]
        public void OptionalMember() {
                Roundtrip(
@"declare var v: {
    member1?;
    member2?: type2;
};
");
        }

        [Test]
        public void EmptyInterface() {
                Roundtrip(
@"interface IFace {
}
");
        }

        [Test]
        public void InterfaceWithMembers() {
                Roundtrip(
@"interface IFace {
    (arg1): returnType1;
    method1(arg2): returnType2;
    new (): returnType3;
    [arg1: string]: returnType4;
    var1: returnType5;
}
");
        }

        [Test]
        public void InterfaceWithExtends() {
                Roundtrip(
@"interface IFace extends module1.Base1, Base2 {
}
");
        }

        [Test]
        public void EmptyModule() {
                Roundtrip(
@"declare module ""myModule"" {
}
");
        }

        [Test]
        public void ModuleWithImports() {
                Roundtrip(
@"declare module ""myModule"" {
    import imp1 = module(""otherModule1"");
    import imp2 = module(""otherModule2"");
}
");
        }

        [Test]
        public void ModuleWithInterface() {
                Roundtrip(
@"declare module ""myModule"" {
    interface MyInterface {
        myMethod();
    }
}
");
        }

        [Test]
        public void ModuleWithExportedInterface() {
                Roundtrip(
@"declare module ""myModule"" {
    export interface MyInterface {
        myMethod();
    }
}
");
        }

        [Test]
        public void ModuleWithMembers() {
                Roundtrip(
@"declare module ""myModule"" {
    var myVar1;
    var myVar2: myType;
    function myFunction() {
    }
    function myFunction2(arg1: someType): someReturnType {
    }
}
");
        }

        [Test]
        public void ModuleWithExportedMembers() {
                Roundtrip(
@"declare module ""myModule"" {
    export var myVar1;
    export var myVar2: myType;
    export function myFunction2(arg1: someType): someReturnType;
    export function myFunction();
}
");
        }

        [Test]
        public void ModuleMemberOrdering() {
                Roundtrip(
@"declare module ""myModule"" {
    export interface IFace2 {
    }
    export var myVar1;
    interface IFace1 {
    }
    var myVar2;
}
");
        }

        [Test]
        public void RoundtripAllNodeTypes() {
                var source = File.ReadAllText("node.d.ts");
                Roundtrip(source);
        }

        [Test, Ignore]
        public void RoundtripRaytracer()
        {
            var source = File.ReadAllText("raytracer.ts");
            Roundtrip(source);
        }
    }
}
