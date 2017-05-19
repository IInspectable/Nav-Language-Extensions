using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language.CodeGen {

    public abstract class CodeModel {

    }

    public abstract class FileGenerationCodeModel : CodeModel {

        protected FileGenerationCodeModel(TaskCodeModel taskCodeModel, string syntaxFilePath, string filePath) {
            SyntaxFilePath = syntaxFilePath ?? String.Empty;
            Task           = taskCodeModel  ?? throw new ArgumentNullException(nameof(taskCodeModel));
            FilePath       = filePath       ?? String.Empty;

        }

        [NotNull]
        public TaskCodeModel Task { get; }
        [NotNull]
        public string SyntaxFilePath { get; }
        [NotNull]
        public string FilePath { get; }               
    }
}