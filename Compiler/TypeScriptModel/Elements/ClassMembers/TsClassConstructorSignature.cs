using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeScriptModel.TypeSystem;
using TypeScriptModel.Visitors;

namespace TypeScriptModel.Elements.ClassMembers
{
    public class TsClassConstructorSignature
    {
        AccessibilityModifier Accessibility; 
        IList<TsParameter> Parameters;

        public TsClassConstructorSignature(AccessibilityModifier accessibility, IList<TsParameter> parameters)
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
