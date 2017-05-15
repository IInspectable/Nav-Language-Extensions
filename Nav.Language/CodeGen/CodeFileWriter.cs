#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeFileWriterResult {

        // TODO Was interessiert uns?
    }

    public class CodeFileWriter {
        public CodeFileWriter(CodeGenerationOptions options) {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [NotNull]
        public CodeGenerationOptions Options { get; }

        public IImmutableList<CodeFileWriterResult> WriteFiles(IEnumerable<CodeGenerationResult> codeGenerationResults) {

            foreach (var codeGenerationResult in codeGenerationResults) {

                var pathProvider = PathProvider.FromTaskDefinition(codeGenerationResult.TaskDefinition);
                
                // TODO ShouldWrite => Force Option etc.
                File.WriteAllText(pathProvider.IBeginWfsInterfaceFile, codeGenerationResult.IBeginWfsCode);
                File.WriteAllText(pathProvider.IWfsInterfaceFile, codeGenerationResult.IWfsCode);
                File.WriteAllText(pathProvider.WfsBaseFile, codeGenerationResult.WfsBaseCode);

                File.WriteAllText(pathProvider.WfsFile, codeGenerationResult.WfsCode);
            }

            // TODO Rückgabewert
            return Enumerable.Empty< CodeFileWriterResult>().ToImmutableList();
        }
    }
}