using System.Collections.Generic;
using TypeScriptModel.Elements;

namespace TypeScriptModel.TypeSystem {
	public class TsModule {
		public string Name { get; private set; }
		public IList<TsModuleImport> Imports { get; private set; }
        public IList<TsTypeMember> ExportedMembers { get; private set; }
        public IList<TsTypeMember> Members { get; private set; }
        public IList<TsInterface> ExportedInterfaces { get; private set; }
        public IList<TsInterface> Interfaces { get; private set; }

		public TsModule(string name, IEnumerable<TsModuleImport> imports, IEnumerable<TsTypeMember> exportedMembers, IEnumerable<TsTypeMember> members, IEnumerable<TsInterface> exportedInterfaces, IEnumerable<TsInterface> interfaces) {
			Name               = name;
			Imports            = new List<TsModuleImport>(imports).AsReadOnly();
			ExportedMembers    = new List<TsTypeMember>(exportedMembers).AsReadOnly();
			Members            = new List<TsTypeMember>(members).AsReadOnly();
			ExportedInterfaces = new List<TsInterface>(exportedInterfaces).AsReadOnly();
			Interfaces         = new List<TsInterface>(interfaces).AsReadOnly();
		}
	}
}