#region Using Directives

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class FileGenerator: Generator {

        public FileGenerator(GenerationOptions options): base(options) {
        }
        
        public IImmutableList<FileGeneratorResult> Generate(CodeGenerationResult codeGenerationResult) {

            if (codeGenerationResult == null) {
                throw new ArgumentNullException(nameof(codeGenerationResult));
            }

            var results = new List<FileGeneratorResult> {
                WriteFile(codeGenerationResult.TaskDefinition, codeGenerationResult.IWfsCodeSpec,      OverwriteCondition.ContentChanged),
                WriteFile(codeGenerationResult.TaskDefinition, codeGenerationResult.IBeginWfsCodeSpec, OverwriteCondition.ContentChanged),
                WriteFile(codeGenerationResult.TaskDefinition, codeGenerationResult.WfsBaseCodeSpec,   OverwriteCondition.ContentChanged),
                WriteFile(codeGenerationResult.TaskDefinition, codeGenerationResult.WfsCodeSpec,       OverwriteCondition.Never, legacyFileName: codeGenerationResult.PathProvider.LegacyWfsFileName)
            };

            foreach(var toCodeSpec in codeGenerationResult.ToCodeSpecs) {
                results.Add(WriteFile(codeGenerationResult.TaskDefinition, toCodeSpec, OverwriteCondition.Never));
            }

            return results.ToImmutableList();
        }

        [NotNull]
        FileGeneratorResult WriteFile(ITaskDefinitionSymbol taskDefinition, CodeGenerationSpec codeGenerationSpec, OverwriteCondition condition, string legacyFileName = null) {

            EnsureDirectory(codeGenerationSpec.FilePath);

            var action = FileGeneratorAction.Skiped;
            if (ShouldWrite(codeGenerationSpec, condition, legacyFileName)) {
                File.WriteAllText(codeGenerationSpec.FilePath, codeGenerationSpec.Content, Encoding.UTF8);
                action = FileGeneratorAction.Updated;
            }

            return new FileGeneratorResult(taskDefinition, action, codeGenerationSpec.FilePath);
        }

        static void EnsureDirectory(string fileName) {
            var dir = Path.GetDirectoryName(fileName);
            // ReSharper disable once AssignNullToNotNullAttribute Lass krachen
            Directory.CreateDirectory(dir);            
        }

        bool ShouldWrite(CodeGenerationSpec codeGenerationSpec, OverwriteCondition condition, string legacyFileName) {

            // Die legacy Datei wird niemals überschrieben/ersetzt.
            var legacyFileExists = legacyFileName != null && File.Exists(legacyFileName);
            if (legacyFileExists) {
                return false;
            }

            // Wenn die Datei nicht existiert, wird sie neu geschrieben
            if (!File.Exists(codeGenerationSpec.FilePath)) {
                return true;
            }

            // Eine Datei mit der Größe 0 gilt als nicht existent, und wird neu geschrieben 
            if (condition == OverwriteCondition.Never) {
                
                var fileInfo = new FileInfo(codeGenerationSpec.FilePath);
                return fileInfo.Length==0;
            }

            // => condition == OverwriteCondition.ContentChanged

            // Das Neuschreiben wurde per Order die Mufti angeordnet
            if (Options.Force) {
                return true;
            }

            // Ansonsten wird die Datei nur neu geschrieben, wenn sich deren Inhalt de facto geändert hat.
            var fileContent = File.ReadAllText(codeGenerationSpec.FilePath);

            return !String.Equals(fileContent, codeGenerationSpec.Content, StringComparison.Ordinal);
        }

        enum OverwriteCondition {
            Never,
            ContentChanged
        }
    }
}