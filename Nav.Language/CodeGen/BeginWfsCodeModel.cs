#region Using Directives

using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class BeginWfsCodeModel: CodeModel {
        
        public BeginWfsCodeModel(ITaskDeclarationSymbol taskDeclaration, IEnumerable<string> namespaces) {

            TaskDeclaration = taskDeclaration;
                        
            Namespaces = namespaces?.OrderBy(ns => ns.Length).ToImmutableList() ?? ImmutableList<string>.Empty;

        }

        public string TaskName {
            get { return TaskDeclaration.Name; }
        }

        [NotNull]
        public ITaskDeclarationSymbol TaskDeclaration { get; set; }

        [NotNull]
        public ImmutableList<string> Namespaces { get; }
    }
}
