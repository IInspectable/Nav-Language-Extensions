#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    abstract class FileGenerationCodeModel : CodeModel {

        protected FileGenerationCodeModel(TaskCodeModel taskCodeModel, string relativeSyntaxFileName, string filePath) {
            RelativeSyntaxFileName = relativeSyntaxFileName ?? String.Empty;
            Task                   = taskCodeModel          ?? throw new ArgumentNullException(nameof(taskCodeModel));
            FilePath               = filePath               ?? String.Empty;
        }

        [NotNull]
        public TaskCodeModel Task { get; }
        [NotNull]
        public string RelativeSyntaxFileName { get; }
        [NotNull]
        public string FilePath { get; }               
    }
}