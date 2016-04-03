using System.Collections.Generic;
using TypeScriptModel.TypeSystem;
using TypescriptMode.Model;
using TypeScriptModel.Expressions;
using TypeScriptModel.Statements;
using TypeScriptModel.Visitors;

namespace TypeScriptModel
{
    using System;
    using System.Globalization;
    using System.Linq;

    using TypeScriptModel.ExtensionMethods;

    public class OutputFormatter : ITypeVisitor<object, bool>, ITypeMemberVisitor<object, bool>, IExpressionVisitor<object, bool>, IStatementVisitor<object, bool>
    {
        private readonly bool _allowIntermediates;

        private CodeBuilder _cb;

        private string _space = " ";

        public OutputFormatter(bool allowIntermediates, bool inline = false)
        {
            _allowIntermediates = allowIntermediates;
            _cb = new CodeBuilder(0, inline);
        }

        public string Format(TsGlobals tsGlobals)
        {
            foreach (var i in tsGlobals.Interfaces)
            {
                i.Accept(this, false);
            }

            foreach (var m in tsGlobals.Modules)
            {
                Format(m);
            }

            return this._cb.ToString();
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

        public static string Format(IList<JsStatement> statements, bool allowIntermediates = false)
        {
            var fmt = new OutputFormatter(allowIntermediates);
            foreach (var statement in statements)
            {
                fmt.VisitStatement(statement, true);
            }
            return fmt._cb.ToString();
        }

        public object VisitExpression(JsExpression expression, bool parenthesized)
        {
            if (parenthesized)
            {
                _cb.Append("(");
            }
            expression.Accept(this, parenthesized);
            if (parenthesized)
            {
                _cb.Append(")");
            }
            return null;
        }

        private void VisitExpressionList(IEnumerable<JsExpression> expressions)
        {
            bool first = true;
            foreach (var x in expressions)
            {
                if (!first)
                    _cb.Append("," + _space);
                VisitExpression(x, GetPrecedence(x.NodeType) >= PrecedenceComma); // We need to parenthesize comma expressions, eg. [1, (2, 3), 4]
                first = false;
            }
        }

        private void FormatGlobalMember(TsTypeMember typeMember, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix))
                _cb.Append(prefix).Append(" ");
            _cb.Append(typeMember is TsMethodSignature ? "function " : "var ");
            typeMember.Accept(this, false);
            _cb.AppendLine();
        }

        public void Format(TsModule module)
        {
            _cb.Append("declare module \"")
              .Append(module.Name)
              .AppendLine("\" {")
              .Indent();

            foreach (var i in module.Imports)
            {
                _cb.Append("import ")
                  .Append(i.Alias)
                  .Append(" = module(\"")
                  .Append(i.Module)
                  .AppendLine("\");");
            }

            foreach (var i in module.ExportedInterfaces)
            {
                _cb.Append("export ");
                i.Accept(this, false);
            }

            foreach (var m in module.ExportedMembers)
            {
                FormatGlobalMember(m, "export");
            }

            foreach (var i in module.Interfaces)
            {
                i.Accept(this, false);
            }

            foreach (var m in module.Members)
            {
                FormatGlobalMember(m, null);
            }

            _cb.Outdent()
              .AppendLine("}");
        }

        private void FormatMemberList(IEnumerable<TsTypeMember> members)
        {
            _cb.AppendLine("{").Indent();
            foreach (var m in members)
            {
                m.Accept(this, false);
                _cb.AppendLine();
            }
            _cb.Outdent().Append("}");
        }

        private void FormatParameter(TsParameter p)
        {
            if (p.ParamArray)
                _cb.Append("...");
            _cb.Append(p.Name);
            if (p.Optional)
                _cb.Append("?");
            if (p.Type != null)
            {
                _cb.Append(": ");
                p.Type.Accept(this, false);
            }
        }

        private void FormatParameterList(IEnumerable<TsParameter> parameters)
        {
            _cb.Append("(");
            if (parameters != null)
            {
                bool first = true;
                foreach (var p in parameters)
                {
                    if (!first)
                    {
                        _cb.Append(", ");
                    }
                    FormatParameter(p);
                    first = false;
                }
            }

            _cb.Append(")");
        }

        public object VisitArrayType(TsArrayType type, bool data)
        {
            type.ElementType.Accept(this, false);
            _cb.Append("[]");
            return null;
        }

        public object VisitObjectType(TsObjectType type, bool inline)
        {
            FormatMemberList(type.Members);
            return null;
        }

        public object VisitFunctionType(TsFunctionType type, bool data)
        {
            FormatTypeParameters(type.TypeParameters);
            FormatParameterList(type.Parameters);
            _cb.Append(" => ");
            type.ReturnType.Accept(this, false);
            return null;
        }

        public object VisitConstructorType(TsConstructorType type, bool data)
        {
            _cb.Append("new ");
            FormatTypeParameters(type.TypeParameters);
            FormatParameterList(type.Parameters);
            _cb.Append(" => ");
            type.ReturnType.Accept(this, false);
            return null;
        }

        private void FormatTypeParameters(IList<TsTypeParameter> typeParameters)
        {
            _cb.Append("<");
            bool first = true;
            foreach (var p in typeParameters)
            {
                if (!first)
                    _cb.Append(", ");
                this.FormatTypeParameter(p);
                first = false;
            }
            _cb.Append(">");
        }

        private void FormatTypeParameter(TsTypeParameter typeParameter)
        {
            _cb.Append(typeParameter.Name);
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
            _cb.Append(type.Name);
            if (type.TypeArgs != null)
            {
                _cb.Append("<");
                bool first = true;
                foreach (var arg in type.TypeArgs)
                {
                    if (!first)
                        _cb.Append(", ");
                    arg.Accept(this, data);
                    first = false;
                }
                _cb.Append(">");
            }
            return null;
        }

        public object VisitInterfaceType(TsInterface iface, bool data)
        {
            _cb.Append("interface ").Append(iface.Name);
            for (int i = 0, n = iface.Extends.Count; i < n; i++)
            {
                _cb.Append(i == 0 ? " extends " : ", ").Append(iface.Extends[i].Name);
            }
            _cb.Append(" ");
            FormatMemberList(iface.Members);
            _cb.AppendLine();
            return null;
        }

        public object VisitPrimitiveType(TsPrimitiveType tsPrimitiveType, bool data)
        {
            switch (tsPrimitiveType.Primitive)
            {
                case TsPrimitive.Any:
                    _cb.Append("any");
                    break;

                case TsPrimitive.Number:
                    _cb.Append("number");
                    break;

                case TsPrimitive.Boolean:
                    _cb.Append("boolean");
                    break;

                case TsPrimitive.String:
                    _cb.Append("string");
                    break;

                case TsPrimitive.Void:
                    _cb.Append("void");
                    break;
            }
            return null;
        }

        public object VisitUnionType(TsUnionType tsUnionType, bool data)
        {
            throw new NotImplementedException();
        }

        public object VisitTupleType(TsTupleType tsTupleType, bool data)
        {
            throw new NotImplementedException();
        }

        public object VisitMethodSignature(TsMethodSignature methodSignature, bool data)
        {
            _cb.Append(methodSignature.Name);
            if(methodSignature.Optional)
            {
                _cb.Append("?");
            }
            FormatCallSignature(methodSignature);
            _cb.Append(";");
            return null;
        }

        public object VisitConstructSignature(TsConstructSignature ctor, bool data)
        {
            _cb.Append("new ");
            FormatCallSignature(ctor);
            _cb.Append(";");
            return null;
        }

        private void FormatCallSignature(IHasCallSignature item)
        {
            if (item.TypeParameters != null)
            {
                FormatTypeParameters(item.TypeParameters);
            }
            FormatParameterList(item.Parameters);
            if (item.ReturnType != null)
            {
                _cb.Append(": ");
                item.ReturnType.Accept(this, false);
            }
        }

        public object VisitIndexSignature(TsIndexSignature indexSignature, bool data)
        {
            _cb.Append("[")
              .Append(indexSignature.ParameterName);
            if (indexSignature.ParameterType != null)
            {
                _cb.Append(": ");
                indexSignature.ParameterType.Accept(this, false);
            }
            _cb.Append("]");
            if (indexSignature.ReturnType != null)
            {
                _cb.Append(": ");
                indexSignature.ReturnType.Accept(this, false);
            }
            _cb.Append(";");
            return null;
        }

        public object VisitPropertySignature(TsPropertySignature propertySignature, bool data)
        {
            _cb.Append(propertySignature.Name);
            if (propertySignature.Optional)
                _cb.Append("?");
            if (propertySignature.Type != null)
            {
                _cb.Append(": ");
                propertySignature.Type.Accept(this, false);
            }
            _cb.Append(";");
            return null;
        }

        public object VisitCallSignature(TsCallSignature callSignature, bool data)
        {
            FormatCallSignature(callSignature);
            _cb.Append(";");
            return null;
        }

        public object VisitArrayLiteralExpression(JsArrayLiteralExpression expression, bool parenthesized)
        {
            _cb.Append("[");
            bool first = true;
            foreach (var x in expression.Elements)
            {
                if (!first)
                    _cb.Append("," + _space);
                if (x != null)
                    VisitExpression(x, GetPrecedence(x.NodeType) >= PrecedenceComma); // We need to parenthesize comma expressions, eg. [1, (2, 3), 4]
                first = false;
            }
            _cb.Append("]");
            return null;
        }

        public object VisitBinaryExpression(JsBinaryExpression expression, bool parenthesized)
        {
            int expressionPrecedence = GetPrecedence(expression.NodeType);
            if (expression.NodeType == ExpressionNodeType.Index)
            {
                VisitExpression(expression.Left, GetPrecedence(expression.Left.NodeType) > expressionPrecedence);
                _cb.Append("[");
                VisitExpression(expression.Right, false);
                _cb.Append("]");
            }
            else
            {
                bool isRightAssociative = expression.NodeType >= ExpressionNodeType.AssignFirst && expression.NodeType <= ExpressionNodeType.AssignLast;
                string spaceBefore = expression.NodeType == ExpressionNodeType.InstanceOf || expression.NodeType == ExpressionNodeType.In ? " " : _space;
                // If minifying, we need to beware of a + +b and a - -b.
                string spaceAfter = (expression.NodeType == ExpressionNodeType.Add && expression.Right.NodeType == ExpressionNodeType.Positive) || (expression.NodeType == ExpressionNodeType.Subtract && expression.Right.NodeType == ExpressionNodeType.Negate) ? " " : spaceBefore;

                VisitExpression(expression.Left, GetPrecedence(expression.Left.NodeType) > expressionPrecedence - (isRightAssociative ? 1 : 0));
                _cb.Append(spaceBefore)
                   .Append(GetBinaryOperatorString(expression.NodeType))
                   .Append(spaceAfter);
                VisitExpression(expression.Right, GetPrecedence(expression.Right.NodeType) > expressionPrecedence - (isRightAssociative ? 0 : 1));
            }
            return null;
        }

        public object VisitCommaExpression(JsCommaExpression expression, bool parenthesized)
        {
            int expressionPrecedence = GetPrecedence(expression.NodeType);
            for (int i = 0; i < expression.Expressions.Count; i++)
            {
                if (i > 0)
                    _cb.Append("," + _space);
                VisitExpression(expression.Expressions[i], GetPrecedence(expression.Expressions[i].NodeType) > expressionPrecedence);
            }
            return null;
        }

        public object VisitConditionalExpression(JsConditionalExpression expression, bool parenthesized)
        {
            // Always parenthesize conditionals (but beware of double parentheses). Better this than accidentally getting the tricky precedence wrong sometimes.
            if (!parenthesized)
                _cb.Append("(");

            // Also, be rather liberal when parenthesizing the operands, partly to avoid bugs, partly for readability.
            VisitExpression(expression.Test, GetPrecedence(expression.Test.NodeType) >= PrecedenceMultiply);
            _cb.Append(_space + "?" + _space);
            VisitExpression(expression.TruePart, GetPrecedence(expression.TruePart.NodeType) >= PrecedenceMultiply);
            _cb.Append(_space + ":" + _space);
            VisitExpression(expression.FalsePart, GetPrecedence(expression.FalsePart.NodeType) >= PrecedenceMultiply);

            if (!parenthesized)
                _cb.Append(")");

            return null;
        }

        public object VisitConstantExpression(JsConstantExpression expression, bool parenthesized)
        {
            switch (expression.NodeType)
            {
                case ExpressionNodeType.Null:
                    _cb.Append("null");
                    break;
                case ExpressionNodeType.Number:
                    _cb.Append(expression.NumberValue.ToString(CultureInfo.InvariantCulture));
                    break;
                case ExpressionNodeType.Regexp:
                    _cb.Append("/" + expression.RegexpValue.Pattern.EscapeJavascriptStringLiteral(true) + "/" + expression.RegexpValue.Options);
                    break;
                case ExpressionNodeType.String:
                    _cb.Append("'" + expression.StringValue.EscapeJavascriptStringLiteral() + "'");
                    break;
                case ExpressionNodeType.Boolean:
                    _cb.Append(expression.BooleanValue ? "true" : "false");
                    break;
                default:
                    throw new ArgumentException("expression");
            }
            return null;
        }

        public object VisitFunctionDefinitionExpression(JsFunctionDefinitionExpression expression, bool parenthesized)
        {
            _cb.Append("function");
            if (expression.Name != null)
                _cb.Append(" ").Append(expression.Name);
            _cb.Append("(");

            bool first = true;
            foreach (var arg in expression.ParameterNames)
            {
                if (!first)
                    _cb.Append("," + _space);
                _cb.Append(arg);
                first = false;
            }
            _cb.Append(")" + _space);
            VisitStatement(expression.Body, false);

            return null;
        }

        public object VisitIdentifierExpression(JsIdentifierExpression expression, bool parenthesized)
        {
            _cb.Append(expression.Name);
            return null;
        }

        public object VisitInvocationExpression(JsInvocationExpression expression, bool parenthesized)
        {
            VisitExpression(expression.Method, GetPrecedence(expression.Method.NodeType) > GetPrecedence(expression.NodeType) || (expression.Method is JsNewExpression)); // Ugly code to make sure that we put parentheses around "new", eg. "(new X())(1)" rather than "new X()(1)"
            _cb.Append("(");
            VisitExpressionList(expression.Arguments);
            _cb.Append(")");
            return null;
        }

        public object VisitObjectLiteralExpression(JsObjectLiteralExpression expression, bool parenthesized)
        {
            if (expression.Values.Count == 0)
            {
                _cb.Append("{}");
            }
            else
            {
                bool multiline = expression.Values.Any(p => p.Value is JsFunctionDefinitionExpression);
                if (multiline)
                    _cb.AppendLine("{").Indent();
                else
                    _cb.Append("{" + _space);

                bool first = true;
                foreach (var v in expression.Values)
                {
                    if (!first)
                    {
                        if (multiline)
                            _cb.AppendLine(",");
                        else
                            _cb.Append("," + _space);
                    }
                    _cb.Append(v.Name.IsValidJavaScriptIdentifier() ? v.Name : ("'" + v.Name.EscapeJavascriptStringLiteral() + "'"))
                       .Append(":" + _space);
                    VisitExpression(v.Value, GetPrecedence(v.Value.NodeType) >= PrecedenceComma); // We ned to parenthesize comma expressions, eg. [1, (2, 3), 4]
                    first = false;
                }
                if (multiline)
                    _cb.AppendLine().Outdent().Append("}");
                else
                    _cb.Append(_space + "}");
            }
            return null;
        }

        public object VisitMemberAccessExpression(JsMemberAccessExpression expression, bool parenthesized)
        {
            VisitExpression(expression.Target, expression.Target.NodeType == ExpressionNodeType.Number || expression.Target.NodeType == ExpressionNodeType.New || ((GetPrecedence(expression.Target.NodeType) > GetPrecedence(expression.NodeType)) && expression.Target.NodeType != ExpressionNodeType.MemberAccess && expression.Target.NodeType != ExpressionNodeType.Invocation)); // Ugly code to ensure that nested typeMember accesses are not parenthesized, but typeMember access nested in new are (and vice versa). Also we need to make sure that we output "(1).X" for that expression.
            _cb.Append(".");
            _cb.Append(expression.MemberName);
            return null;
        }

        public object VisitNewExpression(JsNewExpression expression, bool parenthesized)
        {
            _cb.Append("new ");
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
            VisitExpression(expression.Constructor, needParens);
            _cb.Append("(");
            VisitExpressionList(expression.Arguments);
            _cb.Append(")");
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
            _cb.Append(prefix);
            VisitExpression(expression.Operand, (GetPrecedence(expression.Operand.NodeType) > GetPrecedence(expression.NodeType)) || alwaysParenthesize);
            _cb.Append(postfix);
            return null;
        }

        public object VisitThisExpression(JsThisExpression expression, bool parenthesized)
        {
            _cb.Append("this");
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

        public object VisitStatement(JsStatement statement, bool addNewline)
        {
            return statement.Accept(this, addNewline);
        }

        public object VisitComment(JsComment comment, bool data)
        {
            foreach (var l in comment.Text.Replace("\r", "").Split('\n'))
                _cb.AppendLine("//" + l);
            return null;
        }

        public object VisitBlockStatement(JsBlockStatement statement, bool addNewline)
        {
            _cb.Append("{");
            _cb.AppendLine();
            _cb.Indent();
            foreach (var c in statement.Statements)
                VisitStatement(c, true);
            _cb.Outdent().Append("}");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitBreakStatement(JsBreakStatement statement, bool addNewline)
        {
            _cb.Append("break");
            if (statement.TargetLabel != null)
                _cb.Append(" ").Append(statement.TargetLabel);
            _cb.Append(";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitContinueStatement(JsContinueStatement statement, bool addNewline)
        {
            _cb.Append("continue");
            if (statement.TargetLabel != null)
                _cb.Append(" ").Append(statement.TargetLabel);
            _cb.Append(";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitDoWhileStatement(JsDoWhileStatement statement, bool addNewline)
        {
            _cb.Append("do" + _space);
            VisitStatement(statement.Body, false);
            _cb.Append(_space + "while" + _space + "(");
            VisitExpression(statement.Condition, false);
            _cb.Append(");");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitEmptyStatement(JsEmptyStatement statement, bool addNewline)
        {
            _cb.Append(";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitExpressionStatement(JsExpressionStatement statement, bool addNewline)
        {
            VisitExpression(statement.Expression, statement.Expression is JsFunctionDefinitionExpression);
            _cb.Append(";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitForEachInStatement(JsForEachInStatement statement, bool addNewline)
        {
            _cb.Append("for").Append(_space + "(");
            if (statement.IsLoopVariableDeclared)
                _cb.Append("var ");
            _cb.Append(statement.LoopVariableName)
               .Append(" in ");
            VisitExpression(statement.ObjectToIterateOver, false);
            _cb.Append(")" + _space);
            VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitForStatement(JsForStatement statement, bool addNewline)
        {
            _cb.Append("for").Append(_space + "(");
            VisitStatement(statement.InitStatement, false);

            if (statement.ConditionExpression != null)
            {
                _cb.Append(_space);
                VisitExpression(statement.ConditionExpression, false);
            }
            _cb.Append(";");

            if (statement.IteratorExpression != null)
            {
                _cb.Append(_space);
                VisitExpression(statement.IteratorExpression, false);
            }
            _cb.Append(")" + _space);
            VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitIfStatement(JsIfStatement statement, bool addNewline)
        {
        redo:
            _cb.Append("if").Append(_space + "(");
            VisitExpression(statement.Test, false);
            _cb.Append(")" + _space);
            VisitStatement(statement.Then, (statement.Else != null || addNewline));
            if (statement.Else != null)
            {
                _cb.Append("else");
                if (statement.Else.Statements.Count == 1 && statement.Else.Statements[0] is JsIfStatement)
                {
                    _cb.Append(" ");
                    statement = (JsIfStatement)statement.Else.Statements[0];
                    goto redo;
                }
                else
                    _cb.Append(_space);
            }

            if (statement.Else != null)
                VisitStatement(statement.Else, addNewline);

            return null;
        }

        public object VisitReturnStatement(JsReturnStatement statement, bool addNewline)
        {
            _cb.Append("return");
            if (statement.Value != null)
            {
                _cb.Append(" ");
                VisitExpression(statement.Value, false);
            }
            _cb.Append(";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitSwitchStatement(JsSwitchStatement statement, bool addNewline)
        {
            _cb.Append("switch").Append(_space + "(");
            VisitExpression(statement.Expression, false);
            _cb.Append(")" + _space);
            _cb.Append("{").Indent();
            _cb.AppendLine();
            foreach (var clause in statement.Sections)
            {
                bool first = true;
                foreach (var v in clause.Values)
                {
                    if (!first)
                        _cb.AppendLine();
                    if (v != null)
                    {
                        _cb.Append("case ");
                        VisitExpression(v, false);
                        _cb.Append(":");
                    }
                    else
                    {
                        _cb.Append("default:");
                    }
                    first = false;
                }
                _cb.Append(_space);
                VisitStatement(clause.Body, true);
            }
            _cb.Outdent().Append("}");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitThrowStatement(JsThrowStatement statement, bool addNewline)
        {
            _cb.Append("throw ");
            VisitExpression(statement.Expression, false);
            _cb.Append(";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitTryStatement(JsTryStatement statement, bool addNewline)
        {
            _cb.Append("try" + _space);
            VisitStatement(statement.GuardedStatement, true);
            if (statement.Catch != null)
            {
                _cb.Append("catch")
                   .Append(_space + "(")
                   .Append(statement.Catch.Identifier)
                   .Append(")" + _space);
                VisitStatement(statement.Catch.Body, addNewline || statement.Finally != null);
            }
            if (statement.Finally != null)
            {
                _cb.AppendFormat("finally" + _space);
                VisitStatement(statement.Finally, addNewline);
            }
            return null;
        }

        public object VisitVariableDeclarationStatement(JsVariableDeclarationStatement statement, bool addNewline)
        {
            _cb.Append("var ");
            bool first = true;
            foreach (var d in statement.Declarations)
            {
                if (!first)
                    _cb.Append("," + _space);
                _cb.Append(d.Name);
                if (d.Initializer != null)
                {
                    _cb.Append(_space + "=" + _space);
                    VisitExpression(d.Initializer, false);
                }
                first = false;
            }
            _cb.Append(";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitWhileStatement(JsWhileStatement statement, bool addNewline)
        {
            _cb.Append("while").Append(_space + "(");
            VisitExpression(statement.Condition, false);
            _cb.Append(")" + _space);
            VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitWithStatement(JsWithStatement statement, bool addNewline)
        {
            _cb.Append("with").Append(_space + "(");
            VisitExpression(statement.Object, false);
            _cb.Append(")" + _space);
            VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitLabelledStatement(JsLabelledStatement statement, bool addNewline)
        {
            _cb.Append(statement.Label).Append(":");
            _cb.AppendLine();
            VisitStatement(statement.Statement, addNewline);
            return null;
        }

        public object VisitFunctionStatement(JsFunctionStatement statement, bool addNewline)
        {
            _cb.Append("function " + statement.Name + "(");
            for (int i = 0; i < statement.ParameterNames.Count; i++)
            {
                if (i != 0)
                    _cb.Append("," + _space);
                _cb.Append(statement.ParameterNames[i]);
            }
            _cb.Append(")" + _space);
            VisitStatement(statement.Body, addNewline);
            return null;
        }

        public object VisitGotoStatement(JsGotoStatement statement, bool addNewline)
        {
            if (!_allowIntermediates)
                throw new NotSupportedException("goto should not occur in the output stage");
            _cb.Append("goto ").Append(statement.TargetLabel).Append(";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitYieldStatement(JsYieldStatement statement, bool addNewline)
        {
            if (!_allowIntermediates)
                throw new NotSupportedException("yield should not occur in the output stage");
            if (statement.Value != null)
            {
                _cb.Append("yield return ");
                VisitExpression(statement.Value, false);
                _cb.Append(";");
            }
            else
            {
                _cb.Append("yield break;");
            }
            if (addNewline)
                _cb.AppendLine();
            return null;
        }

        public object VisitAwaitStatement(JsAwaitStatement statement, bool addNewline)
        {
            if (!_allowIntermediates)
                throw new NotSupportedException("await should not occur in the output stage");
            _cb.Append("await ");
            VisitExpression(statement.Awaiter, false);
            _cb.Append(":" + statement.OnCompletedMethodName + ";");
            if (addNewline)
                _cb.AppendLine();
            return null;
        }
    }
}
