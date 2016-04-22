using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TypeScriptModel.ExtensionMethods;
using TypeScriptModel.Statements;

namespace TypeScriptModel.Expressions {
    using TypeScriptModel.TypeSystem;
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsFunctionDefinitionExpression : JsExpression, IHasCallSignature {
        public IList<TsParameter> Parameters { get; private set; }        
        public IList<TsTypeParameter> TypeParameters { get; private set; }
        public TsType ReturnType { get; private set; }
        public JsBlockStatement Body { get; private set; }

        /// <summary>
        /// Null if the function does not have a name.
        /// </summary>
        public string Name { get; private set; }

        internal JsFunctionDefinitionExpression(IEnumerable<string> parameterNames, JsStatement body, string name = null, TsType type = null)
            : this(null, parameterNames.Select(n => new TsParameter(n, null, false, false, null)).ToList(), body, name, type)
        {
        }
        internal JsFunctionDefinitionExpression(IList<TsTypeParameter> TypeParameters, IList<TsParameter> parameters, JsStatement body, string name = null, TsType type = null)
            : base(ExpressionNodeType.FunctionDefinition)
        {
            if (body == null) throw new ArgumentNullException("body");
            if (name != null && !name.IsValidJavaScriptIdentifier()) throw new ArgumentException("name");

            Parameters = parameters ?? new List<TsParameter>();
			if (Parameters.Any(n => !n.Name.IsValidJavaScriptIdentifier()))
				throw new ArgumentException("parameterNames");
            Body = JsBlockStatement.MakeBlock(body);
            ReturnType = type;
            Name = name;
        }

        public override TReturn Accept<TReturn, TData>(IExpressionVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitFunctionDefinitionExpression(this, data);
        }
    }
}
