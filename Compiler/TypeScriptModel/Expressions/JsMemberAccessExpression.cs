using System;
using TypeScriptModel.ExtensionMethods;

namespace TypeScriptModel.Expressions {
    using TypeScriptModel.Visitors;

    /// <summary>
    /// A MemberExpression represents an elements which applies on a previous Expression
    /// </summary>
    [Serializable]
    public class JsMemberAccessExpression : JsExpression {
        public const string Prototype = "prototype";

        public JsExpression Target { get; private set; }
        public string MemberName { get; private set; }

        internal JsMemberAccessExpression(JsExpression target, string memberName) : base(ExpressionNodeType.MemberAccess) {
            if (target == null) throw new ArgumentNullException("target");
            if (memberName == null) throw new ArgumentNullException("memberName");
            if (!memberName.IsValidJavaScriptIdentifier()) throw new ArgumentException("member");

            MemberName = memberName;
            Target = target;
        }

        public override TReturn Accept<TReturn, TData>(IExpressionVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitMemberAccessExpression(this, data);
        }
    }
}
