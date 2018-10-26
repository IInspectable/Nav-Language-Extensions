#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class FindReferencesArgs {

        public FindReferencesArgs(ISymbol originatingSymbol,
                                  CodeGenerationUnit originatingCodeGenerationUnit,
                                  NavSolution solution,
                                  IFindReferencesContext context) {

            OriginatingSymbol             = originatingSymbol             ?? throw new ArgumentNullException(nameof(originatingSymbol));
            OriginatingCodeGenerationUnit = originatingCodeGenerationUnit ?? throw new ArgumentNullException(nameof(originatingCodeGenerationUnit));
            Context                       = context                       ?? throw new ArgumentNullException(nameof(context));
            Solution                      = solution                      ?? throw new ArgumentNullException(nameof(solution));

        }

        [NotNull]
        public ISymbol OriginatingSymbol { get; }

        [NotNull]
        public CodeGenerationUnit OriginatingCodeGenerationUnit { get; }

        [NotNull]
        public NavSolution Solution { get; }

        [NotNull]
        public IFindReferencesContext Context { get; }

    }

}