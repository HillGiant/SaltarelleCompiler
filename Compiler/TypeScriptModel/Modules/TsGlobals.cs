using System.Collections.Generic;

namespace TypeScriptModel.TypeSystem {
    using TypeScriptModel.Expressions;

    public class TsGlobals {
        public IList<TsModule> Modules { get; private set; }
        public IList<TsInterfaceType> Interfaces { get; private set; }
        public IList<JsExpression> Expressions { get; private set; }

		public TsGlobals(IEnumerable<TsModule> modules, IEnumerable<TsInterfaceType> interfaces, IEnumerable<JsExpression> expressions ) {
			Modules    = new List<TsModule>(modules).AsReadOnly();
			Interfaces = new List<TsInterfaceType>(interfaces).AsReadOnly();
            Expressions = new List<JsExpression>(expressions).AsReadOnly();
		}
	}
}