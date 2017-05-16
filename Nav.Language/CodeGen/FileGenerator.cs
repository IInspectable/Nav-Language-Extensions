#region Using Directives

using System;
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

            var results = new List<FileGeneratorResult> {
                Write(codeGenerationResult.TaskDefinition, codeGenerationResult.IWfsCode,      OverwriteCondition.ContentChanged, pathProvider.IWfsInterfaceFile),
                Write(codeGenerationResult.TaskDefinition, codeGenerationResult.IBeginWfsCode, OverwriteCondition.ContentChanged, pathProvider.IBeginWfsInterfaceFile),
                Write(codeGenerationResult.TaskDefinition, codeGenerationResult.WfsBaseCode,   OverwriteCondition.ContentChanged, pathProvider.WfsBaseFile),
                Write(codeGenerationResult.TaskDefinition, codeGenerationResult.WfsCode,       OverwriteCondition.Never         , pathProvider.WfsFile, alternateFileName: pathProvider.OldWfsFile)
            };

            return results.ToImmutableList();
        }

        [NotNull]
        FileGeneratorResult Write(ITaskDefinitionSymbol taskDefinition, string content, OverwriteCondition condition, string fileName, string alternateFileName = null) {

            EnsureDirectory(fileName);

            var action = FileGeneratorAction.Skiped;
            if (ShouldWrite(content, condition, fileName, alternateFileName)) {
                File.WriteAllText(fileName, content);
                action = FileGeneratorAction.Updated;
            }

            return new FileGeneratorResult(taskDefinition, action, fileName);
        }

        static void EnsureDirectory(string fileName) {
            var dir = Path.GetDirectoryName(fileName);
            // ReSharper disable once AssignNullToNotNullAttribute Lass krachen
            Directory.CreateDirectory(dir);            
        }

        bool ShouldWrite(string content, OverwriteCondition condition, string fileName, string alternateFileName) {

            // Die alternative Datei wird niemals überschrieben (legacy code!).
            var alternateFileExists = alternateFileName != null && File.Exists(alternateFileName);
            if (alternateFileExists) {
                return false;
            }

            // Wenn die Datei nicht existiert, wird sie neu geschrieben
            if (!File.Exists(fileName)) {
                return true;
            }

            // Eine Datei mit der Größe 0 gilt als nicht existent, und wird neu geschrieben 
            if (condition == OverwriteCondition.Never) {
                
                var fileInfo = new FileInfo(fileName);
                return fileInfo.Length==0;
            }

            // => condition == OverwriteCondition.ContentChanged

            // Das Neuschreiben wurde per Order die Mufti angeordnet
            if (Options.Force) {
                return true;
            }

            // Ansonsten wird die Datei nur neu geschrieben, wenn sich deren Inhalt de facto geändert hat.
            var fileContent = File.ReadAllText(fileName);

            return !String.Equals(fileContent, content, StringComparison.Ordinal);
        }

        enum OverwriteCondition {
            Never,
            ContentChanged
        }
    }
}