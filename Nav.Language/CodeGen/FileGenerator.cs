#region Using Directives

using System.IO;
using System.Linq;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class FileGeneratorResult {

        // TODO Was interessiert uns?
    }

    public class FileGenerator: Generator {

        public FileGenerator(GenerationOptions options): base(options) {
        }
        
        public IImmutableList<FileGeneratorResult> Generate(CodeGenerationResult codeGenerationResult) {

            var pathProvider = PathProvider.FromTaskDefinition(codeGenerationResult.TaskDefinition);
                
            // TODO ShouldWrite => Force Option etc.
            Write(pathProvider.IBeginWfsInterfaceFile, codeGenerationResult.IBeginWfsCode);
            Write(pathProvider.IWfsInterfaceFile, codeGenerationResult.IWfsCode);
            Write(pathProvider.WfsBaseFile, codeGenerationResult.WfsBaseCode);

            // Spezial Handling für Wfs File
            Write(pathProvider.WfsFile, codeGenerationResult.WfsCode);

            // TODO Rückgabewert
            return Enumerable.Empty< FileGeneratorResult>().ToImmutableList();
        }

        void Write(string fileName, string content) {
            var dir = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(dir)) {
                // ReSharper disable once AssignNullToNotNullAttribute Lass krachen
                Directory.CreateDirectory(dir);
            }

            // TODO ShouldWrite => Force Option etc.
            File.WriteAllText(fileName, content);
        }
    }
}