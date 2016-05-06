﻿using TypeScriptModel.TypeSystem.Parameters;

namespace TypeScriptModel.Visitors
{
    using TypeScriptModel.TypeSystem.TypeMembers;

    public interface ITypeMemberVisitor<out TReturn, in TData> 
    {
        TReturn VisitMethodSignature(TsMethodSignature member, TData data);
        TReturn VisitConstructSignature(TsConstructSignature member, TData data);
        TReturn VisitIndexSignature(TsIndexSignature member, TData data);
        TReturn VisitPropertySignature(TsPropertySignature member, TData data);
        TReturn VisitCallSignature(TsCallSignature tsCallSignature, TData data);
    }
}
