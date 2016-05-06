namespace TypeScriptModel.TypeSystem.Types
{
    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.Visitors;

    public enum TsPrimitive
    {
        Any,

        Number,

        Boolean,

        String,

        Void
    }

    public class TsPrimitiveType : TsType
    {
        public static TsPrimitiveType Any = new TsPrimitiveType(TsPrimitive.Any);

        public static TsPrimitiveType Number = new TsPrimitiveType(TsPrimitive.Number);

        public static TsPrimitiveType Boolean = new TsPrimitiveType(TsPrimitive.Boolean);

        public static TsPrimitiveType String = new TsPrimitiveType(TsPrimitive.String);

        public static TsPrimitiveType Void = new TsPrimitiveType(TsPrimitive.Void);

        public TsPrimitive Primitive { get; private set; }

        private TsPrimitiveType(TsPrimitive primitive)
        {
            this.Primitive = primitive;
        }

        public override TReturn Accept<TReturn, TData>(ITypeVisitor<TReturn, TData> visitor, TData data)
        {
            return visitor.VisitPrimitiveType(this, data);
        }
    }
}
