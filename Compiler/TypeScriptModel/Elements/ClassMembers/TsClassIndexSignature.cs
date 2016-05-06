namespace TypeScriptModel.Elements.ClassMembers
{
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.TypeMembers;
    using TypeScriptModel.Visitors;

    public class TsClassIndexSignature : TsClassMember
    {
        public  TsIndexSignature Signature;

        public TsClassIndexSignature(TsIndexSignature signature)
        {
            Signature = signature;
        }

        public override TReturn Accept<TReturn, TData>(IClassMemberVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitClassIndexSignature(this, data);
        }
    }
}
