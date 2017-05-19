using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language.CodeGen {

    public abstract class CodeModel {

    }

    public abstract class FileGenerationCodeModel : CodeModel {

        protected FileGenerationCodeModel(string syntaxFilePath, TaskCodeModel taskCodeModel, string filePath) {
            SyntaxFilePath = syntaxFilePath ?? String.Empty;
            Task           = taskCodeModel  ?? throw new ArgumentNullException(nameof(taskCodeModel));
            FilePath       = filePath       ?? String.Empty;

        }

        [NotNull]
        public string SyntaxFilePath { get; }
        [NotNull]
        public TaskCodeModel Task { get; }
        [NotNull]
        public string FilePath { get; }
    }
}