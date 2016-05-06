namespace TypeScriptModel.TypeSystem
{
    using System.Collections.Generic;

    using TypeScriptModel.TypeSystem.Parameters;
    using TypeScriptModel.TypeSystem.Types;

    // lolcat unintentional but hilarious
    public interface IHasCallSignature
    {
        TsType ReturnType { get; }
        IList<TsTypeParameter> TypeParameters { get; }
        IList<TsParameter> Parameters { get; }
    }
}
