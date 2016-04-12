using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeScriptModel;
using TypeScriptModel.Elements;

namespace TypeScriptParser.Tests
{
    static class TestUtils
    {
        public static T ParseElement<T>(string source) where T : TsSourceElement
        {
            var expr = Parser.ParseSourceElement(source, new ThrowingErrorReporter());
            Assert.That(expr, Is.InstanceOf<T>());
            return (T)expr;
        }

        public static void RoundtripElement(string source, string expected = null)
        {
            var element = Parser.ParseSourceElement(source, new ThrowingErrorReporter());
            SerializedTypeMatchesInput(source, element);
        }

        public static void SerializedTypeMatchesInput(string source, TsSourceElement type)
        {
            Assert.That(OutputFormatter.Format(type, false).Replace("\r\n", "\n").Replace("\t", "    "), Is.EqualTo(source.Replace("\r\n", "\n").Replace("\t", "    ")));
        }
    }
}
