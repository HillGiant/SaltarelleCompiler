﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TypeScriptModel.ExtensionMethods;
using TypeScriptModel.Statements;

namespace TypeScriptModel.Expressions {
    using TypeScriptModel.TypeSystem;
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsFunctionDefinitionExpression : JsExpression {
        public ReadOnlyCollection<string> ParameterNames { get; private set; }
        public TsType Type { get; private set; }
        public JsBlockStatement Body { get; private set; }

        /// <summary>
        /// Null if the function does not have a name.
        /// </summary>
        public string Name { get; private set; }

        internal JsFunctionDefinitionExpression(IEnumerable<string> parameterNames, JsStatement body, string name = null, TsType type = null) : base(ExpressionNodeType.FunctionDefinition) {
            if (parameterNames == null) throw new ArgumentNullException("parameterNames");
            if (body == null) throw new ArgumentNullException("body");
            if (name != null && !name.IsValidJavaScriptIdentifier()) throw new ArgumentException("name");

            ParameterNames = parameterNames.AsReadOnly();
			if (ParameterNames.Any(n => !n.IsValidJavaScriptIdentifier()))
				throw new ArgumentException("parameterNames");
            Body = JsBlockStatement.MakeBlock(body);
            Type = type;
            Name = name;
        }

        public override TReturn Accept<TReturn, TData>(IExpressionVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitFunctionDefinitionExpression(this, data);
        }
    }
}
