namespace TypeScriptModel.TypeSystem
{
    using System.Collections.Generic;

    // lolcat unintentional but hilarious
    public interface IHasCallSignature
    {
        TsType ReturnType { get; }
        IList<TsTypeParameter> TypeParameters { get; }
        IList<TsParameter> Parameters { get; }
    }
}
