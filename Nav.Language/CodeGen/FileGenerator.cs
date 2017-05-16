#region Using Directives

using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class FileGenerator: Generator {

        public FileGenerator(GenerationOptions options): base(options) {
        }
        
        public IImmutableList<FileGeneratorResult> Generate(CodeGenerationResult codeGenerationResult) {

            var pathProvider = PathProvider.FromTaskDefinition(codeGenerationResult.TaskDefinition);

            var results = new List<FileGeneratorResult>();

            results.Add(Write(codeGenerationResult.TaskDefinition, codeGenerationResult.IWfsCode     , OverwriteCondition.ContentChanged, pathProvider.IWfsInterfaceFile));
            results.Add(Write(codeGenerationResult.TaskDefinition, codeGenerationResult.IBeginWfsCode, OverwriteCondition.ContentChanged, pathProvider.IBeginWfsInterfaceFile));
            results.Add(Write(codeGenerationResult.TaskDefinition, codeGenerationResult.WfsBaseCode  , OverwriteCondition.ContentChanged, pathProvider.WfsBaseFile));
            // Spezial Handling für Wfs File
            // TODO OldWfsFile berücksichtigen!!!
            results.Add(Write(codeGenerationResult.TaskDefinition, codeGenerationResult.WfsCode      , OverwriteCondition.Never         , pathProvider.WfsFile));

            return results.ToImmutableList();
        }

        [NotNull]
        FileGeneratorResult Write(ITaskDefinitionSymbol taskDefinition, string content, OverwriteCondition condition, string fileName) {
            var dir = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(dir)) {
                // ReSharper disable once AssignNullToNotNullAttribute Lass krachen
                Directory.CreateDirectory(dir);
            }
            
            var action = FileGeneratorAction.Skiped;
            if (ShouldWrite(content, condition, fileName)) {
                File.WriteAllText(fileName, content);
                action = FileGeneratorAction.Updated;
            }

            return new FileGeneratorResult(taskDefinition, action, fileName);
        }

        enum OverwriteCondition {
            Never,
            ContentChanged
        }

        bool ShouldWrite(string content, OverwriteCondition condition, string fileName) {

            if (!File.Exists(fileName)) {
                return true;
            }
            
            if (condition == OverwriteCondition.Never) {
                // Eine Datei mit der Größe 0 gilt als nicht existent
                var fileInfo = new FileInfo(fileName);
                return fileInfo.Length==0;
            }

            if (Options.Force) {
                return true;
            }

            var oldContent = File.ReadAllText(fileName);

            return oldContent != content;
        }
    }
}