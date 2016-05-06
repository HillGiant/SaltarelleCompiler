namespace TypeScriptModel.TypeSystem
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using TypescriptMode.Model;

    using TypeScriptModel.Elements;
    using TypeScriptModel.Elements.ClassMembers;
    using TypeScriptModel.Expressions;
    using TypeScriptModel.ExtensionMethods;
    using TypeScriptModel.Statements;
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.TypeMembers;
    using TypeScriptModel.TypeSystem.Types;
    using TypeScriptModel.Visitors;

    public class OutputFormatter : ITypeVisitor<object, bool>, ITypeMemberVisitor<object, bool>, IExpressionVisitor<object, bool>, IStatementVisitor<object, bool>, ISourceElementVisitor<object, bool>, IClassMemberVisitor<object, bool>
    {
        private readonly bool _allowIntermediates;

        private CodeBuilder _cb;

        private string _space = " ";

        public OutputFormatter(bool allowIntermediates, bool inline = false)
        {
            this._allowIntermediates = allowIntermediates;
            this._cb = new CodeBuilder(0, inline);
        }

        public static string Format(TsSourceElement element, bool allowIntermediates = false)
        {
            var fmt = new OutputFormatter(allowIntermediates);
            element.Accept(fmt, false);
            return fmt._cb.ToString();
        }

        public static string Format(TsType type, bool allowIntermediates = false)
        {
            var fmt = new OutputFormatter(allowIntermediates);
            type.Accept(fmt, false);
            return fmt._cb.ToString();
        }

        public static string Format(JsExpression expression, bool allowIntermediates = false)
        {
            var fmt = new OutputFormatter(allowIntermediates);
            fmt.VisitExpression(expression, false);
            return fmt._cb.ToString();
        }

        public static string Format(JsStatement statement, bool allowIntermediates = false)
        {
            var fmt = new OutputFormatter(allowIntermediates);
            fmt.VisitStatement(statement, true);
            return fmt._cb.ToString();
        }

        public static string Format(IList<TsSourceElement> elements, bool allowIntermediates = false)
        {
            var fmt = new OutputFormatter(allowIntermediates);
            foreach (var element in elements)
            {
                fmt.VisitElement(element, false);
                fmt._cb.AppendLine();
            }
            return fmt._cb.ToString();
        }

        public object VisitExpression(JsExpression expression, bool parenthesized)
        {
            if (parenthesized)
            {
                this._cb.Append("(");
            }
            expression.Accept(this, parenthesized);
            if (parenthesized)
            {
                this._cb.Append(")");
            }
            return null;
        }

        private void VisitExpressionList(IEnumerable<JsExpression> expressions)
        {
            bool first = true;
            foreach (var x in expressions)
            {
                if (!first)
                    this._cb.Append("," + this._space);
                this.VisitExpression(x, GetPrecedence(x.NodeType) >= PrecedenceComma); // We need to parenthesize comma expressions, eg. [1, (2, 3), 4]
                first = false;
            }
        }

        private void FormatTypeMemberList(IEnumerable<TsTypeMember> members)
        {
            this._cb.AppendLine("{").Indent();
            foreach (var m in members)
            {
                m.Accept(this, false);
                this._cb.AppendLine();
            }
            this._cb.Outdent().Append("}");
        }

        private void FormatClassMemberList(IEnumerable<TsClassMember> members)
        {
            this._cb.AppendLine("{").Indent();
            foreach (var m in members)
            {
                m.Accept(this, false);
                this._cb.AppendLine();
            }
            this._cb.Outdent().Append("}");
        }

        private void FormatParameter(TsParameter p)
        {
            this._cb.Append(FormatAccessibility(p.Modifier));
            if (p.ParamArray)
                this._cb.Append("...");
            this._cb.Append(p.Name);
            if (p.Optional)
                this._cb.Append("?");
            if (p.Type != null)
            {
                this._cb.Append(": ");
                p.Type.Accept(this, false);
            }
        }

        private void FormatParameterList(IEnumerable<TsParameter> parameters)
        {
            this._cb.Append("(");
            if (parameters != null)
            {
                bool first = true;
                foreach (var p in parameters)
                {
                    if (!first)
                    {
                        this._cb.Append(", ");
                    }
                    this.FormatParameter(p);
                    first = false;
                }
            }

            this._cb.Append(")");
        }

        public object VisitArrayType(TsArrayType type, bool data)
        {
            type.ElementType.Accept(this, false);
            this._cb.Append("[]");
            return null;
        }

        public object VisitObjectType(TsObjectType type, bool inline)
        {
            this.FormatTypeMemberList(type.Members);
            return null;
        }

        public object VisitFunctionType(TsFunctionType type, bool data)
        {
            if (type.TypeParameters != null)
            {
                this.FormatTypeParameters(type.TypeParameters);
            }
            this.FormatParameterList(type.Parameters);
            this._cb.Append(" => ");
            type.ReturnType.Accept(this, false);
            return null;
        }

        public object VisitConstructorType(TsConstructorType type, bool data)
        {
            this._cb.Append("new ");
            if (type.TypeParameters != null)
            {
                this.FormatTypeParameters(type.TypeParameters);
            }
            this.FormatParameterList(type.Parameters);
            this._cb.Append(" => ");
            type.ReturnType.Accept(this, false);
            return null;
        }

        private void FormatTypeParameters(IList<TsTypeParameter> typeParameters)
        {
            this._cb.Append("<");
            bool first = true;
            foreach (var p in typeParameters)
            {
                if (!first)
                    this._cb.Append(", ");
                this.FormatTypeParameter(p);
                first = false;
            }
            this._cb.Append(">");
        }

        private void FormatTypeParameter(TsTypeParameter typeParameter)
        {
            this._cb.Append(typeParameter.Name);
            if (typeParameter.Constraint != null)
            {
                this._cb.Append(" extends ");
                var inlineFormatter = new OutputFormatter(false, true);
                typeParameter.Constraint.Accept(inlineFormatter, false);
                this._cb.Append(inlineFormatter._cb.ToString());
            }
        }

        public object VisitTypeReference(TsTypeReference type, bool data)
        {
            this._cb.Append(type.Name);
            if (type.TypeArgs != null)
            {
                this._cb.Append("<");
                bool first = true;
                foreach (var arg in type.TypeArgs)
                {
                    if (!first)
                    {
                        this._cb.Append(", ");
                    }
                    arg.Accept(this, data);
                    first = false;
                }
                this._cb.Append(">");
            }
            return null;
        }

        public object VisitInterface(TsInterface iface, bool data)
        {
            this._cb.Append("interface ").Append(iface.Name);
            if (iface.TypeParameters != null)
            {
                this.FormatTypeParameters(iface.TypeParameters);
            }
            if (iface.Extends != null)
            {
                this._cb.Append(" extends ");
                bool first = true;
                foreach (var extend in iface.Extends)
                {
                    if (!first)
                    {
                        this._cb.Append(", ");
                    }
                    extend.Accept(this, data);
                    first = false;
                }
            }
            this._cb.Append(" ");
            this.FormatTypeMemberList(iface.Members);
            return null;
        }

        public object VisitStatementElement(TsStatementElement s, bool data)
        {
            return s.Statement.Accept(this, data);
        }

        public object VisitClass(TsClass tsClass, bool data)
        {
            this._cb.Append("class ").Append(tsClass.Name);
            if (tsClass.TypeParameters != null)
            {
                this.FormatTypeParameters(tsClass.TypeParameters);
            }
            if (tsClass.Extends != null)
            {
                this._cb.Append(" extends ");
                bool first = true;
                foreach (var extend in tsClass.Extends)
                {
                    if (!first)
                    {
                        this._cb.Append(", ");
                    }
                    extend.Accept(this, data);
                    first = false;
                }
            }
            if (tsClass.Implements != null)
            {
                this._cb.Append(" implements ");
                bool first = true;
                foreach (var implement in tsClass.Implements)
                {
                    if (!first)
                    {
                        this._cb.Append(", ");
                    }
                    implement.Accept(this, data);
                    first = false;
                }
            }
            this._cb.Append(" ");
            this.FormatClassMemberList(tsClass.Members);
            this._cb.AppendLine();
            return null;
        }

        public object VisitModule(TsModule tsModule, bool data)
        {
            this._cb.Append("module \"").Append(tsModule.Name).Append("\"");
            this._cb.AppendLine(" {").Indent();
            foreach (var e in tsModule.Elements)
            {
                e.Accept(this, false);
                 this._cb.AppendLine();
            }
            this._cb.Outdent().Append("}");
            return null;
        }

        public object VisitExport(TsExportElement tsExportElement, bool data)
        {
            this._cb.Append("export ");
            tsExportElement.Exported.Accept(this, data);
            return null;
        }

        public object VisitAmbientDeclaration(TsAmbientDeclaration tsAmbientDeclaration, bool data)
        {
            this._cb.Append("declare ");
            tsAmbientDeclaration.Declared.Accept(this, data);
            return null;
        }

        public object VisitPrimitiveType(TsPrimitiveType tsPrimitiveType, bool data)
        {
            switch (tsPrimitiveType.Primitive)
            {
                case TsPrimitive.Any:
                    this._cb.Append("any");
                    break;

                case TsPrimitive.Number:
                    this._cb.Append("number");
                    break;

                case TsPrimitive.Boolean:
                    this._cb.Append("boolean");
                    break;

                case TsPrimitive.String:
                    this._cb.Append("string");
                    break;

                case TsPrimitive.Void:
                    this._cb.Append("void");
                    break;
            }
            return null;
        }

        public object VisitUnionType(TsUnionType tsUnionType, bool data)
        {
            throw new NotImplementedException();
        }

        public object VisitTupleType(TsTupleType tuple, bool data)
        {
            this._cb.Append("[");
            bool first = true;
            foreach (var t in tuple.Types)
            {
                if (!first)
                {
                    this._cb.Append(", ");
                }
                t.Accept(this, data);
                first = false;
            }

            this._cb.Append("]");
            return null;
        }

        public object VisitMethodSignature(TsMethodSignature methodSignature, bool data)
        {
            this._cb.Append(methodSignature.Name);
            if(methodSignature.Optional)
            {
                this._cb.Append("?");
            }
            FormatCallSignature(methodSignature);
            this._cb.Append(";");
            return null;
        }

        public object VisitAmbientFunctionDeclaration(TsAmbientFunctionDeclaration function, bool data)
        {
            this._cb.Append("function ");
            this._cb.Append(function.Name);
            FormatCallSignature(function);
            this._cb.Append(";");
            return null;
        }

        public object VisitConstructSignature(TsConstructSignature ctor, bool data)
        {
            this._cb.Append("new ");
            FormatCallSignature(ctor);
            this._cb.Append(";");
            return null;
        }

        private void FormatCallSignature(IHasCallSignature item)
        {
            if (item.TypeParameters != null)
            {
                this.FormatTypeParameters(item.TypeParameters);
            }
            this.FormatParameterList(item.Parameters);
            if (item.ReturnType != null)
            {
                this._cb.Append(": ");
                item.ReturnType.Accept(this, false);
            }
        }

        public object VisitIndexSignature(TsIndexSignature indexSignature, bool data)
        {
            this._cb.Append("[")
              .Append(indexSignature.ParameterName);
            if (indexSignature.ParameterType != null)
            {
                this._cb.Append(": ");
                indexSignature.ParameterType.Accept(this, false);
            }
            this._cb.Append("]");
            if (indexSignature.ReturnType != null)
            {
                this._cb.Append(": ");
                indexSignature.ReturnType.Accept(this, false);
            }
            this._cb.Append(";");
            return null;
        }

        public object VisitPropertySignature(TsPropertySignature propertySignature, bool data)
        {
            this._cb.Append(propertySignature.Name);
            if (propertySignature.Optional)
                this._cb.Append("?");
            if (propertySignature.Type != null)
            {
                this._cb.Append(": ");
                propertySignature.Type.Accept(this, false);
            }
            this._cb.Append(";");
            return null;
        }

        public object VisitCallSignature(TsCallSignature callSignature, bool data)
        {
            FormatCallSignature(callSignature);
            this._cb.Append(";");
            return null;
        }

        public object VisitArrayLiteralExpression(JsArrayLiteralExpression expression, bool parenthesized)
        {
            this._cb.Append("[");
            bool first = true;
            foreach (var x in expression.Elements)
            {
                if (!first)
                    this._cb.Append("," + this._space);
                if (x != null)
                    this.VisitExpression(x, GetPrecedence(x.NodeType) >= PrecedenceComma); // We need to parenthesize comma expressions, eg. [1, (2, 3), 4]
                first = false;
            }
            this._cb.Append("]");
            return null;
        }

        public object VisitBinaryExpression(JsBinaryExpression expression, bool parenthesized)
        {
            int expressionPrecedence = GetPrecedence(expression.NodeType);
            if (expression.NodeType == ExpressionNodeType.Index)
            {
                this.VisitExpression(expression.Left, GetPrecedence(expression.Left.NodeType) > expressionPrecedence);
                this._cb.Append("[");
                this.VisitExpression(expression.Right, false);
                this._cb.Append("]");
            }
            else
            {
                bool isRightAssociative = expression.NodeType >= ExpressionNodeType.AssignFirst && expression.NodeType <= ExpressionNodeType.AssignLast;
                string spaceBefore = expression.NodeType == ExpressionNodeType.InstanceOf || expression.NodeType == ExpressionNodeType.In ? " " : this._space;
                // If minifying, we need to beware of a + +b and a - -b.
                string spaceAfter = (expression.NodeType == ExpressionNodeType.Add && expression.Right.NodeType == ExpressionNodeType.Positive) || (expression.NodeType == ExpressionNodeType.Subtract && expression.Right.NodeType == ExpressionNodeType.Negate) ? " " : spaceBefore;

                this.VisitExpression(expression.Left, GetPrecedence(expression.Left.NodeType) > expressionPrecedence - (isRightAssociative ? 1 : 0));
                this._cb.Append(spaceBefore)
                   .Append(GetBinaryOperatorString(expression.NodeType))
                   .Append(spaceAfter);
                this.VisitExpression(expression.Right, GetPrecedence(expression.Right.NodeType) > expressionPrecedence - (isRightAssociative ? 0 : 1));
            }
            return null;
        }

        public object VisitCommaExpression(JsCommaExpression expression, bool parenthesized)
        {
            int expressionPrecedence = GetPrecedence(expression.NodeType);
            for (int i = 0; i < expression.Expressions.Count; i++)
            {
                if (i > 0)
                    this._cb.Append("," + this._space);
                this.VisitExpression(expression.Expressions[i], GetPrecedence(expression.Expressions[i].NodeType) > expressionPrecedence);
            }
            return null;
        }

        public object VisitConditionalExpression(JsConditionalExpression expression, bool parenthesized)
        {
            // Always parenthesize conditionals (but beware of double parentheses). Better this than accidentally getting the tricky precedence wrong sometimes.
            if (!parenthesized)
                this._cb.Append("(");

            // Also, be rather liberal when parenthesizing the operands, partly to avoid bugs, partly for readability.
            this.VisitExpression(expression.Test, GetPrecedence(expression.Test.NodeType) >= PrecedenceMultiply);
            this._cb.Append(this._space + "?" + this._space);
            this.VisitExpression(expression.TruePart, GetPrecedence(expression.TruePart.NodeType) >= PrecedenceMultiply);
            this._cb.Append(this._space + ":" + this._space);
            this.VisitExpression(expression.FalsePart, GetPrecedence(expression.FalsePart.NodeType) >= PrecedenceMultiply);

            if (!parenthesized)
                this._cb.Append(")");

            return null;
        }

        public object VisitConstantExpression(JsConstantExpression expression, bool parenthesized)
        {
            switch (expression.NodeType)
            {
                case ExpressionNodeType.Null:
                    this._cb.Append("null");
                    break;
                case ExpressionNodeType.Number:
                    this._cb.Append(expression.NumberValue.ToString(CultureInfo.InvariantCulture));
                    break;
                case ExpressionNodeType.Regexp:
                    this._cb.Append("/" + expression.RegexpValue.Pattern.EscapeJavascriptStringLiteral(true) + "/" + expression.RegexpValue.Options);
                    break;
                case ExpressionNodeType.String:
                    this._cb.Append("'" + expression.StringValue.EscapeJavascriptStringLiteral() + "'");
                    break;
                case ExpressionNodeType.Boolean:
                    this._cb.Append(expression.BooleanValue ? "true" : "false");
                    break;
                default:
                    throw new ArgumentException("expression");
            }
            return null;
        }

        public object VisitFunctionDefinitionExpression(JsFunctionDefinitionExpression expression, bool parenthesized)
        {
            this._cb.Append("function");
            if (expression.Name != null)
                this._cb.Append(" ").Append(expression.Name);
            FormatCallSignature(expression);
            this.VisitStatement(expression.Body, false);

            return null;
        }

        public object VisitIdentifierExpression(JsIdentifierExpression expression, bool parenthesized)
        {
            this._cb.Append(expression.Name);
            return null;
        }

        public object VisitInvocationExpression(JsInvocationExpression expression, bool parenthesized)
        {
            this.VisitExpression(expression.Method, GetPrecedence(expression.Method.NodeType) > GetPrecedence(expression.NodeType) || (expression.Method is JsNewExpression)); // Ugly code to make sure that we put parentheses around "new", eg. "(new X())(1)" rather than "new X()(1)"
            this._cb.Append("(");
            this.VisitExpressionList(expression.Arguments);
            this._cb.Append(")");
            return null;
        }

        public object VisitObjectLiteralExpression(JsObjectLiteralExpression expression, bool parenthesized)
        {
            if (expression.Values.Count == 0)
            {
                this._cb.Append("{}");
            }
            else
            {
                bool multiline = expression.Values.Any(p => p.Value is JsFunctionDefinitionExpression);
                if (multiline)
                    this._cb.AppendLine("{").Indent();
                else
                    this._cb.Append("{" + this._space);

                bool first = true;
                foreach (var v in expression.Values)
                {
                    if (!first)
                    {
                        if (multiline)
                            this._cb.AppendLine(",");
                        else
                            this._cb.Append("," + this._space);
                    }
                    this._cb.Append(v.Name.IsValidJavaScriptIdentifier() ? v.Name : ("'" + v.Name.EscapeJavascriptStringLiteral() + "'"))
                       .Append(":" + this._space);
                    this.VisitExpression(v.Value, GetPrecedence(v.Value.NodeType) >= PrecedenceComma); // We ned to parenthesize comma expressions, eg. [1, (2, 3), 4]
                    first = false;
                }
                if (multiline)
                    this._cb.AppendLine().Outdent().Append("}");
                else
                    this._cb.Append(this._space + "}");
            }
            return null;
        }

        public object VisitMemberAccessExpression(JsMemberAccessExpression expression, bool parenthesized)
        {
            this.VisitExpression(expression.Target, expression.Target.NodeType == ExpressionNodeType.Number || expression.Target.NodeType == ExpressionNodeType.New || ((GetPrecedence(expression.Target.NodeType) > GetPrecedence(expression.NodeType)) && expression.Target.NodeType != ExpressionNodeType.MemberAccess && expression.Target.NodeType != ExpressionNodeType.Invocation)); // Ugly code to ensure that nested typeMember accesses are not parenthesized, but typeMember access nested in new are (and vice versa). Also we need to make sure that we output "(1).X" for that expression.
            this._cb.Append(".");
            this._cb.Append(expression.MemberName);
            return null;
        }

        public object VisitNewExpression(JsNewExpression expression, bool parenthesized)
        {
            this._cb.Append("new ");
            bool needParens = GetPrecedence(expression.Constructor.NodeType) >= PrecedenceMemberOrNewOrInvocation;
            if (expression.Constructor.NodeType == ExpressionNodeType.MemberAccess)
            {
                // We don't need to parenthesize something like new a.b.c()
                JsExpression expr = expression.Constructor;
                for (; ; )
                {
                    if (GetPrecedence(expr.NodeType) < PrecedenceMemberOrNewOrInvocation)
                    {
                        needParens = false;
                        break;
                    }
                    else if (expr.NodeType == ExpressionNodeType.MemberAccess)
                    {
                        expr = ((JsMemberAccessExpression)expr).Target;
                    }
                    else
                        break;
                }
            }
            this.VisitExpression(expression.Constructor, needParens);
            this._cb.Append("(");
            this.VisitExpressionList(expression.Arguments);
            this._cb.Append(")");
            return null;
        }

        public object VisitUnaryExpression(JsUnaryExpression expression, bool parenthesized)
        {
            string prefix = "", postfix = "";
            bool alwaysParenthesize = false;
            switch (expression.NodeType)
            {
                case ExpressionNodeType.PrefixPlusPlus: prefix = "++"; break;
                case ExpressionNodeType.PrefixMinusMinus: prefix = "--"; break;
                case ExpressionNodeType.PostfixPlusPlus: postfix = "++"; break;
                case ExpressionNodeType.PostfixMinusMinus: postfix = "--"; break;
                case ExpressionNodeType.LogicalNot: prefix = "!"; break;
                case ExpressionNodeType.BitwiseNot: prefix = "~"; break;
                case ExpressionNodeType.Positive: prefix = "+"; break;
                case ExpressionNodeType.Negate: prefix = "-"; break;
                case ExpressionNodeType.TypeOf: prefix = "typeof"; alwaysParenthesize = true; break;
                case ExpressionNodeType.Void: prefix = "void"; alwaysParenthesize = true; break;
                case ExpressionNodeType.Delete: prefix = "delete "; break;
                default: throw new ArgumentException("expression");
            }
            this._cb.Append(prefix);
            this.VisitExpression(expression.Operand, (GetPrecedence(expression.Operand.NodeType) > GetPrecedence(expression.NodeType)) || alwaysParenthesize);
            this._cb.Append(postfix);
            return null;
        }

        public object VisitThisExpression(JsThisExpression expression, bool parenthesized)
        {
            this._cb.Append("this");
            return null;
        }

        private static string GetBinaryOperatorString(ExpressionNodeType oper)
        {
            switch (oper)
            {
                case ExpressionNodeType.Multiply: return "*";
                case ExpressionNodeType.Divide: return "/";
                case ExpressionNodeType.Modulo: return "%";
                case ExpressionNodeType.Add: return "+";
                case ExpressionNodeType.Subtract: return "-";
                case ExpressionNodeType.LeftShift: return "<<";
                case ExpressionNodeType.RightShiftSigned: return ">>";
                case ExpressionNodeType.RightShiftUnsigned: return ">>>";
                case ExpressionNodeType.Lesser: return "<";
                case ExpressionNodeType.LesserOrEqual: return "<=";
                case ExpressionNodeType.Greater: return ">";
                case ExpressionNodeType.GreaterOrEqual: return ">=";
                case ExpressionNodeType.In: return "in";
                case ExpressionNodeType.InstanceOf: return "instanceof";
                case ExpressionNodeType.Equal: return "==";
                case ExpressionNodeType.NotEqual: return "!=";
                case ExpressionNodeType.Same: return "===";
                case ExpressionNodeType.NotSame: return "!==";
                case ExpressionNodeType.BitwiseAnd: return "&";
                case ExpressionNodeType.BitwiseXor: return "^";
                case ExpressionNodeType.BitwiseOr: return "|";
                case ExpressionNodeType.LogicalAnd: return "&&";
                case ExpressionNodeType.LogicalOr: return "||";
                case ExpressionNodeType.Assign: return "=";
                case ExpressionNodeType.MultiplyAssign: return "*=";
                case ExpressionNodeType.DivideAssign: return "/=";
                case ExpressionNodeType.ModuloAssign: return "%=";
                case ExpressionNodeType.AddAssign: return "+=";
                case ExpressionNodeType.SubtractAssign: return "-=";
                case ExpressionNodeType.LeftShiftAssign: return "<<=";
                case ExpressionNodeType.RightShiftSignedAssign: return ">>=";
                case ExpressionNodeType.RightShiftUnsignedAssign: return ">>>=";
                case ExpressionNodeType.BitwiseAndAssign: return "&=";
                case ExpressionNodeType.BitwiseOrAssign: return "|=";
                case ExpressionNodeType.BitwiseXorAssign: return "^=";
                default:
                    throw new InvalidOperationException("Invalid operator " + oper.ToString());
            }
        }

        private const int PrecedenceTerminal = 0;
        private const int PrecedenceMemberOrNewOrInvocation = PrecedenceTerminal + 1;
        private const int PrecedenceFunctionDefinition = PrecedenceMemberOrNewOrInvocation + 1; // The methodSignature definition precedence is kind of strange. methodSignature() {}(x) does not invoke the methodSignature, although I guess this is due to semicolon insertion rather than precedence. Cheating with the precedence solves the problem.
        private const int PrecedenceIncrDecr = PrecedenceFunctionDefinition + 1;
        private const int PrecedenceOtherUnary = PrecedenceIncrDecr + 1;
        private const int PrecedenceMultiply = PrecedenceOtherUnary + 1;
        private const int PrecedenceAddition = PrecedenceMultiply + 1;
        private const int PrecedenceBitwiseShift = PrecedenceAddition + 1;
        private const int PrecedenceRelational = PrecedenceBitwiseShift + 1;
        private const int PrecedenceEquality = PrecedenceRelational + 1;
        private const int PrecedenceBitwiseAnd = PrecedenceEquality + 1;
        private const int PrecedenceBitwiseXor = PrecedenceBitwiseAnd + 1;
        private const int PrecedenceBitwiseOr = PrecedenceBitwiseXor + 1;
        private const int PrecedenceLogicalAnd = PrecedenceBitwiseOr + 1;
        private const int PrecedenceLogicalOr = PrecedenceLogicalAnd + 1;
        private const int PrecedenceConditional = PrecedenceLogicalOr + 1;
        private const int PrecedenceAssignment = PrecedenceConditional + 1;
        private const int PrecedenceComma = PrecedenceAssignment + 1;
        private const int PrecedenceLiteral = PrecedenceComma + 1;

        private static int GetPrecedence(ExpressionNodeType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionNodeType.ArrayLiteral:
                    return PrecedenceTerminal;

                case ExpressionNodeType.LogicalAnd:
                    return PrecedenceLogicalAnd;

                case ExpressionNodeType.LogicalOr:
                    return PrecedenceLogicalOr;

                case ExpressionNodeType.NotEqual:
                case ExpressionNodeType.Equal:
                case ExpressionNodeType.Same:
                case ExpressionNodeType.NotSame:
                    return PrecedenceEquality;

                case ExpressionNodeType.LesserOrEqual:
                case ExpressionNodeType.GreaterOrEqual:
                case ExpressionNodeType.Lesser:
                case ExpressionNodeType.Greater:
                case ExpressionNodeType.InstanceOf:
                case ExpressionNodeType.In:
                    return PrecedenceRelational;

                case ExpressionNodeType.Subtract:
                case ExpressionNodeType.Add:
                    return PrecedenceAddition;

                case ExpressionNodeType.Modulo:
                case ExpressionNodeType.Divide:
                case ExpressionNodeType.Multiply:
                    return PrecedenceMultiply;

                case ExpressionNodeType.BitwiseAnd:
                    return PrecedenceBitwiseAnd;

                case ExpressionNodeType.BitwiseOr:
                    return PrecedenceBitwiseOr;

                case ExpressionNodeType.BitwiseXor:
                    return PrecedenceBitwiseXor;

                case ExpressionNodeType.LeftShift:
                case ExpressionNodeType.RightShiftSigned:
                case ExpressionNodeType.RightShiftUnsigned:
                    return PrecedenceBitwiseShift;

                case ExpressionNodeType.Assign:
                case ExpressionNodeType.MultiplyAssign:
                case ExpressionNodeType.DivideAssign:
                case ExpressionNodeType.ModuloAssign:
                case ExpressionNodeType.AddAssign:
                case ExpressionNodeType.SubtractAssign:
                case ExpressionNodeType.LeftShiftAssign:
                case ExpressionNodeType.RightShiftSignedAssign:
                case ExpressionNodeType.RightShiftUnsignedAssign:
                case ExpressionNodeType.BitwiseAndAssign:
                case ExpressionNodeType.BitwiseOrAssign:
                case ExpressionNodeType.BitwiseXorAssign:
                    return PrecedenceAssignment;

                case ExpressionNodeType.Comma:
                    return PrecedenceComma;

                case ExpressionNodeType.Conditional:
                    return PrecedenceConditional;

                case ExpressionNodeType.Number:
                case ExpressionNodeType.String:
                case ExpressionNodeType.Regexp:
                case ExpressionNodeType.Null:
                case ExpressionNodeType.Boolean:
                    return PrecedenceTerminal;

                case ExpressionNodeType.FunctionDefinition:
                    return PrecedenceFunctionDefinition;

                case ExpressionNodeType.Identifier:
                case ExpressionNodeType.This:
                    return PrecedenceTerminal;

                case ExpressionNodeType.MemberAccess:
                case ExpressionNodeType.New:
                case ExpressionNodeType.Index:
                case ExpressionNodeType.Invocation:
                    return PrecedenceMemberOrNewOrInvocation;

                case ExpressionNodeType.ObjectLiteral:
                    return PrecedenceTerminal;

                case ExpressionNodeType.PrefixPlusPlus:
                case ExpressionNodeType.PrefixMinusMinus:
                case ExpressionNodeType.PostfixPlusPlus:
                case ExpressionNodeType.PostfixMinusMinus:
                    return PrecedenceIncrDecr;

                case ExpressionNodeType.TypeOf:
                case ExpressionNodeType.LogicalNot:
                case ExpressionNodeType.Negate:
                case ExpressionNodeType.Positive:
                case ExpressionNodeType.Delete:
                case ExpressionNodeType.Void:
                case ExpressionNodeType.BitwiseNot:
                    return PrecedenceOtherUnary;

                case ExpressionNodeType.TypeReference:
                    return PrecedenceTerminal;

                case ExpressionNodeType.Literal:
                    return PrecedenceLiteral;

                default:
                    throw new ArgumentException("nodeType");
            }
        }

        public object VisitElement(TsSourceElement element, bool addNewline)
        {
            return element.Accept(this, addNewline);
        }

        public object VisitStatement(JsStatement statement, bool addNewline)
        {
            return statement.Accept(this, addNewline);
        }

        public object VisitComment(JsComment comment, bool data)
        {
            foreach (var l in comment.Text.Replace("\r", "").Split('\n'))
                this._cb.AppendLine("//" + l);
            return null;
        }

        public object VisitBlockStatement(JsBlockStatement statement, bool addNewline)
        {
            this._cb.Append("{");
            this._cb.AppendLine();
            this._cb.Indent();
            foreach (var c in statement.Statements)
                this.VisitStatement(c, true);
            this._cb.Outdent().Append("}");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitBreakStatement(JsBreakStatement statement, bool addNewline)
        {
            this._cb.Append("break");
            if (statement.TargetLabel != null)
                this._cb.Append(" ").Append(statement.TargetLabel);
            this._cb.Append(";");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitContinueStatement(JsContinueStatement statement, bool addNewline)
        {
            this._cb.Append("continue");
            if (statement.TargetLabel != null)
                this._cb.Append(" ").Append(statement.TargetLabel);
            this._cb.Append(";");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitDoWhileStatement(JsDoWhileStatement statement, bool addNewline)
        {
            this._cb.Append("do" + this._space);
            this.VisitStatement(statement.Body, false);
            this._cb.Append(this._space + "while" + this._space + "(");
            this.VisitExpression(statement.Condition, false);
            this._cb.Append(");");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitEmptyStatement(JsEmptyStatement statement, bool addNewline)
        {
            this._cb.Append(";");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitExpressionStatement(JsExpressionStatement statement, bool addNewline)
        {
            this.VisitExpression(statement.Expression, statement.Expression is JsFunctionDefinitionExpression);
            this._cb.Append(";");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitForEachInStatement(JsForEachInStatement statement, bool addNewline)
        {
            this._cb.Append("for").Append(this._space + "(");
            if (statement.IsLoopVariableDeclared)
                this._cb.Append("var ");
            this._cb.Append(statement.LoopVariableName)
               .Append(" in ");
            this.VisitExpression(statement.ObjectToIterateOver, false);
            this._cb.Append(")" + this._space);
            this.VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitForStatement(JsForStatement statement, bool addNewline)
        {
            this._cb.Append("for").Append(this._space + "(");
            this.VisitStatement(statement.InitStatement, false);

            if (statement.ConditionExpression != null)
            {
                this._cb.Append(this._space);
                this.VisitExpression(statement.ConditionExpression, false);
            }
            this._cb.Append(";");

            if (statement.IteratorExpression != null)
            {
                this._cb.Append(this._space);
                this.VisitExpression(statement.IteratorExpression, false);
            }
            this._cb.Append(")" + this._space);
            this.VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitIfStatement(JsIfStatement statement, bool addNewline)
        {
        redo:
            this._cb.Append("if").Append(this._space + "(");
            this.VisitExpression(statement.Test, false);
            this._cb.Append(")" + this._space);
            this.VisitStatement(statement.Then, (statement.Else != null || addNewline));
            if (statement.Else != null)
            {
                this._cb.Append("else");
                if (statement.Else.Statements.Count == 1 && statement.Else.Statements[0] is JsIfStatement)
                {
                    this._cb.Append(" ");
                    statement = (JsIfStatement)statement.Else.Statements[0];
                    goto redo;
                }
                else
                    this._cb.Append(this._space);
            }

            if (statement.Else != null)
                this.VisitStatement(statement.Else, addNewline);

            return null;
        }

        public object VisitReturnStatement(JsReturnStatement statement, bool addNewline)
        {
            this._cb.Append("return");
            if (statement.Value != null)
            {
                this._cb.Append(" ");
                this.VisitExpression(statement.Value, false);
            }
            this._cb.Append(";");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitSwitchStatement(JsSwitchStatement statement, bool addNewline)
        {
            this._cb.Append("switch").Append(this._space + "(");
            this.VisitExpression(statement.Expression, false);
            this._cb.Append(")" + this._space);
            this._cb.Append("{").Indent();
            this._cb.AppendLine();
            foreach (var clause in statement.Sections)
            {
                bool first = true;
                foreach (var v in clause.Values)
                {
                    if (!first)
                        this._cb.AppendLine();
                    if (v != null)
                    {
                        this._cb.Append("case ");
                        this.VisitExpression(v, false);
                        this._cb.Append(":");
                    }
                    else
                    {
                        this._cb.Append("default:");
                    }
                    first = false;
                }
                this._cb.Append(this._space);
                this.VisitStatement(clause.Body, true);
            }
            this._cb.Outdent().Append("}");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitThrowStatement(JsThrowStatement statement, bool addNewline)
        {
            this._cb.Append("throw ");
            this.VisitExpression(statement.Expression, false);
            this._cb.Append(";");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitTryStatement(JsTryStatement statement, bool addNewline)
        {
            this._cb.Append("try" + this._space);
            this.VisitStatement(statement.GuardedStatement, true);
            if (statement.Catch != null)
            {
                this._cb.Append("catch")
                   .Append(this._space + "(")
                   .Append(statement.Catch.Identifier)
                   .Append(")" + this._space);
                this.VisitStatement(statement.Catch.Body, addNewline || statement.Finally != null);
            }
            if (statement.Finally != null)
            {
                this._cb.AppendFormat("finally" + this._space);
                this.VisitStatement(statement.Finally, addNewline);
            }
            return null;
        }

        public object VisitVariableDeclarationStatement(JsVariableDeclarationStatement statement, bool addNewline)
        {
            this._cb.Append("var ");
            bool first = true;
            foreach (var d in statement.Declarations)
            {
                if (!first)
                    this._cb.Append("," + this._space);
                this._cb.Append(d.Name);

                if (d.Type != null)
                {
                    this._cb.Append(":" + this._space);
                    d.Type.Accept(this, addNewline);
                }

                if (d.Initializer != null)
                {
                    this._cb.Append(this._space + "=" + this._space);
                    this.VisitExpression(d.Initializer, false);
                }
                first = false;
            }
            this._cb.Append(";");
            if (addNewline)
                this._cb.AppendLine();
            return null;
        }

        public object VisitWhileStatement(JsWhileStatement statement, bool addNewline)
        {
            this._cb.Append("while").Append(this._space + "(");
            this.VisitExpression(statement.Condition, false);
            this._cb.Append(")" + this._space);
            this.VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitWithStatement(JsWithStatement statement, bool addNewline)
        {
            this._cb.Append("with").Append(this._space + "(");
            this.VisitExpression(statement.Object, false);
            this._cb.Append(")" + this._space);
            this.VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitLabelledStatement(JsLabelledStatement statement, bool addNewline)
        {
            this._cb.Append(statement.Label).Append(":");
            this._cb.AppendLine();
            this.VisitStatement(statement.Statement, addNewline);
            return null;
        }

        public object VisitFunctionStatement(JsFunctionStatement statement, bool addNewline)
        {
            this._cb.Append("function " + statement.Name);
            FormatCallSignature(statement);
            if(statement.Body != null)
            {
                this.VisitStatement(statement.Body, addNewline);
            }
            else
            {
                this._cb.Append(";");
            }
            return null;
        }

        public object VisitGotoStatement(JsGotoStatement statement, bool addNewline)
        {
            if (!this._allowIntermediates)
            {
                throw new NotSupportedException("goto should not occur in the output stage");
            }
            this._cb.Append("goto ").Append(statement.TargetLabel).Append(";");
            if (addNewline)
            {
                this._cb.AppendLine();
            }
            return null;
        }

        public object VisitYieldStatement(JsYieldStatement statement, bool addNewline)
        {
            if (!this._allowIntermediates)
                throw new NotSupportedException("yield should not occur in the output stage");
            if (statement.Value != null)
            {
                this._cb.Append("yield return ");
                this.VisitExpression(statement.Value, false);
                this._cb.Append(";");
            }
            else
            {
                this._cb.Append("yield break;");
            }
            if (addNewline)
            {
                this._cb.AppendLine();
            }
            return null;
        }

        public object VisitAwaitStatement(JsAwaitStatement statement, bool addNewline)
        {
            if (!this._allowIntermediates)
            {
                throw new NotSupportedException("await should not occur in the output stage");
            }
            this._cb.Append("await ");
            this.VisitExpression(statement.Awaiter, false);
            this._cb.Append(":" + statement.OnCompletedMethodName + ";");
            if (addNewline)
            {
                this._cb.AppendLine();
            }
            return null;
        }

        public object VisitConstructorDeclaration(TsConstructorDeclaration tsConstructorDeclaration, bool data)
        {
            TsClassConstructorSignature last = tsConstructorDeclaration.Signatures.Last();
            foreach (var signature in tsConstructorDeclaration.Signatures)
            {
                signature.Accept(this, data);
                if (signature != last)
                {
                    this._cb.AppendLine(";");
                }
            }
            tsConstructorDeclaration.Body.Accept(this, data);
            return null;
        }

        public object VisitClassConstructorSignature(TsClassConstructorSignature tsClassConstructorSignature, bool data)
        {
            if (tsClassConstructorSignature.Accessibility != null)
            {
                this._cb.Append(FormatAccessibility(tsClassConstructorSignature.Accessibility));
            }
            this._cb.Append("constructor");
            this.FormatParameterList(tsClassConstructorSignature.Parameters);
            return null;
        }

        public object VisitMethodDeclaration(TsMethodDeclaration tsMethodDeclaration, bool data)
        {
            TsClassMethodSignature last = tsMethodDeclaration.Signatures.Last();
            foreach (var signature in tsMethodDeclaration.Signatures)
            {
                signature.Accept(this, data);
                if (signature != last)
                {
                    this._cb.AppendLine(";");
                }
            }
            tsMethodDeclaration.Body.Accept(this, data);
            return null;
        }

        public object VisitClassMethodSignature(TsClassMethodSignature tsClassMethodSignature, bool data)
        {
            if (tsClassMethodSignature.Accessibility != null)
            {
                this._cb.Append(FormatAccessibility(tsClassMethodSignature.Accessibility));
            }
            if (tsClassMethodSignature.IsStatic)
            {
                this._cb.Append("static ");
            }
            this._cb.Append(tsClassMethodSignature.Name);
            FormatCallSignature(tsClassMethodSignature);
            return null;
        }

        public object VisitClassMemberDeclaration(TsClassMemberDeclaration tsClassMemberDeclaration, bool addNewline)
        {
            if (tsClassMemberDeclaration.Accessibility != null)
            {
                this._cb.Append(FormatAccessibility(tsClassMemberDeclaration.Accessibility));
            }
            if (tsClassMemberDeclaration.IsStatic)
            {
                this._cb.Append("static ");
            }
            this._cb.Append(tsClassMemberDeclaration.VariableDeclaration.Name);

            if (tsClassMemberDeclaration.VariableDeclaration.Type != null)
            {
                this._cb.Append(":" + this._space);
                tsClassMemberDeclaration.VariableDeclaration.Type.Accept(this, addNewline);
            }

            if (tsClassMemberDeclaration.VariableDeclaration.Initializer != null)
            {
                this._cb.Append(this._space + "=" + this._space);
                this.VisitExpression(tsClassMemberDeclaration.VariableDeclaration.Initializer, false);
            }
            this._cb.Append(";");

            return null;
        }

        public object VisitClassIndexSignature(TsClassIndexSignature tsClassIndexSignaure, bool data)
        {
            return tsClassIndexSignaure.Signature.Accept(this, data);
        }

        public object VisitClassSetAccessor(TsClassSetAccessor tsClassSetAccessor, bool data)
        {
            if (tsClassSetAccessor.Accessibility != null)
            {
                this._cb.Append(FormatAccessibility(tsClassSetAccessor.Accessibility));
            }
            if (tsClassSetAccessor.IsStatic)
            {
                this._cb.Append("static ");
            }
            this._cb.Append("set ");
            this._cb.Append(tsClassSetAccessor.Name);
            this.FormatParameterList(new List<TsParameter>{tsClassSetAccessor.Parameter});
            if (tsClassSetAccessor.TypeAnnotation != null)
            {
                this._cb.Append(": ");
                tsClassSetAccessor.TypeAnnotation.Accept(this, false);
            }
            tsClassSetAccessor.Body.Accept(this, data);
            return null;
        }

        public object VisitClassGetAccessor(TsClassGetAccessor tsClassGetAccessor, bool data)
        {
            if (tsClassGetAccessor.Accessibility != null)
            {
                this._cb.Append(FormatAccessibility(tsClassGetAccessor.Accessibility));
            }
            if (tsClassGetAccessor.IsStatic)
            {
                this._cb.Append("static ");
            }
            this._cb.Append("get ");
            this._cb.Append(tsClassGetAccessor.Name);
            this._cb.Append("()");
            if (tsClassGetAccessor.TypeAnnotation != null)
            {
                this._cb.Append(": ");
                tsClassGetAccessor.TypeAnnotation.Accept(this, false);
            }
            tsClassGetAccessor.Body.Accept(this, data);
            return null;
        }

        public string FormatAccessibility(AccessibilityModifier? m)
        {
            if (m != null)
            {
                switch (m)
                {
                    case AccessibilityModifier.Public:
                        return "public ";
                    case AccessibilityModifier.Private:
                        return "private ";
                    case AccessibilityModifier.Protected:
                        return "protected ";
                }
            }
            
            return string.Empty;
        }

        public object VisitImportDeclaration(TsImportDeclaration tsImportDeclaration, bool data)
        {
            this._cb.Append("import ").Append(tsImportDeclaration.Alias).Append(" = module(\"").Append(tsImportDeclaration.Module).Append("\");");
            return null;
        }

        public object VisitNamespace(TsNamespace tsNamespace, bool data)
        {
            this._cb.Append("module ").Append(tsNamespace.Name);
            this._cb.AppendLine(" {").Indent();
            foreach (var e in tsNamespace.Elements)
            {
                e.Accept(this, false);
                this._cb.AppendLine();
            }
            this._cb.Outdent().Append("}");
            return null;
        }
    }
}
