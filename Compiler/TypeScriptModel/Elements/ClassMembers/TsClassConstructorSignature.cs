using System.Collections.Generic;
using TypeScriptModel.TypeSystem;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.Elements.ClassMembers
{
    public class TsClassConstructorSignature
    {
        public AccessibilityModifier? Accessibility;
        public IList<TsParameter> Parameters;

        public TsClassConstructorSignature(AccessibilityModifier? accessibility, IList<TsParameter> parameters)
        {
            Accessibility = accessibility;
            Parameters = parameters;
        }

        public TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClassConstructorSignature(this, data);
        }
    }
}
