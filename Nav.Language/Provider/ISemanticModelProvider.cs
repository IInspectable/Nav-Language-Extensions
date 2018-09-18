#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public interface ISemanticModelProvider: IDisposable {

        [NotNull]
        CodeGenerationUnit GetSemanticModel(CodeGenerationUnitSyntax syntax);

    }

}