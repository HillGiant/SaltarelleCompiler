using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem {
	public class TsModule {
		public string Name { get; private set; }
		public IList<TsModuleImport> Imports { get; private set; }
        public IList<TsMember> ExportedMembers { get; private set; }
        public IList<TsMember> Members { get; private set; }
        public IList<TsInterface> ExportedInterfaces { get; private set; }
        public IList<TsInterface> Interfaces { get; private set; }

		public TsModule(string name, IEnumerable<TsModuleImport> imports, IEnumerable<TsMember> exportedMembers, IEnumerable<TsMember> members, IEnumerable<TsInterface> exportedInterfaces, IEnumerable<TsInterface> interfaces) {
			Name               = name;
			Imports            = new List<TsModuleImport>(imports).AsReadOnly();
			ExportedMembers    = new List<TsMember>(exportedMembers).AsReadOnly();
			Members            = new List<TsMember>(members).AsReadOnly();
			ExportedInterfaces = new List<TsInterface>(exportedInterfaces).AsReadOnly();
			Interfaces         = new List<TsInterface>(interfaces).AsReadOnly();
		}
	}
}