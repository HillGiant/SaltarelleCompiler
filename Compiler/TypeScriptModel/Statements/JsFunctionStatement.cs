﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptModel.Statements
{
    using TypeScriptModel.ExtensionMethods;
    using TypeScriptModel.TypeSystem;
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.Types;
    using TypeScriptModel.Visitors;

    [Serializable]
    public class JsFunctionStatement : JsStatement, IHasCallSignature {
        public string Name { get; private set; }

        public IList<TsTypeParameter> TypeParameters { get; private set; }

        public IList<TsParameter> Parameters { get; private set; }
        public JsBlockStatement Body { get; private set; }
        public TsType ReturnType { get; private set; }

        public JsFunctionStatement(string name, IEnumerable<string> parameterNames, JsStatement body, TsType type = null)
            : this(null, parameterNames.Select(n => new TsParameter(n, null, false, false, null)).ToList(), name, body,  type)
        {
        }
        public JsFunctionStatement(IList<TsTypeParameter> typeParameters, IList<TsParameter> parameters, string name, JsStatement body, TsType type = null)
        {
            if (!name.IsValidJavaScriptIdentifier()) throw new ArgumentException("name");
            //if (body == null) throw new ArgumentNullException("body");
            TypeParameters = typeParameters;
            Parameters = parameters ?? new List<TsParameter>();
			if (Parameters.Any(n => !n.Name.IsValidJavaScriptIdentifier()))
				throw new ArgumentException("parameters");
            Body = JsBlockStatement.MakeBlock(body);
            Name = name;
            ReturnType = type;
        }

        public override TReturn Accept<TReturn, TData>(IStatementVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitFunctionStatement(this, data);
        }
    }
}
