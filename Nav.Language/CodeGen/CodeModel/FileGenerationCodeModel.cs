#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    abstract class FileGenerationCodeModel : CodeModel {

        protected FileGenerationCodeModel(TaskCodeInfo taskCodeInfo, string relativeSyntaxFileName, string filePath) {
            RelativeSyntaxFileName = relativeSyntaxFileName ?? String.Empty;
            Task                   = taskCodeInfo           ?? throw new ArgumentNullException(nameof(taskCodeInfo));
            FilePath               = filePath               ?? String.Empty;
        }

        [NotNull]
        public TaskCodeInfo Task { get; }
        [NotNull]
        public string RelativeSyntaxFileName { get; }
        [NotNull]
        public string FilePath { get; }               
    }
}