namespace TypeScriptModel.TypeSystem {
	public class TsModuleImport {
		public string Module { get; private set; }
		public string Alias { get; private set; }

		public TsModuleImport(string module, string alias) {
			Module = module;
			Alias  = alias;
		}
	}
}