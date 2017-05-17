#region Using Directives

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public sealed partial class NavCodeGeneratorPipeline {

        [CanBeNull]
        readonly ILogger _logger;

        public NavCodeGeneratorPipeline(GenerationOptions options, ILogger logger) {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        [NotNull]
        public GenerationOptions Options { get; }

        public bool Run(IEnumerable<FileSpec> fileSpecs) {

            var logger         = new LoggerAdapter(_logger);
            var modelGenerator = new CodeModelGenerator(Options);
            var codeGenerator  = new CodeGenerator(Options);
            var fileGenerator  = new FileGenerator(Options);
            var statistic      = new Statistic();

            logger.LogProcessBegin();

            foreach (var fileSpec in fileSpecs) {

                statistic.IncrementFileCount();

                logger.LogProcessFileBegin(fileSpec);

                if (!File.Exists(fileSpec.FilePath)) {
                    logger.LogError(String.Format(DiagnosticDescriptors.Semantic.Nav0004File0NotFound.MessageFormat, fileSpec));
                    continue;
                }
                
                var syntax = SyntaxTree.FromFile(fileSpec.FilePath);
                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax((CodeGenerationUnitSyntax)syntax.GetRoot());

                if (logger.LogErrors(fileSpec, syntax.Diagnostics) || logger.LogErrors(fileSpec, codeGenerationUnit.Diagnostics)) {
                    continue;
                }

                logger.LogWarnings(fileSpec, syntax.Diagnostics);
                logger.LogWarnings(fileSpec, codeGenerationUnit.Diagnostics);
                
                var codeModelResults = modelGenerator.Generate(codeGenerationUnit);
                foreach (var codeModelResult in codeModelResults) {

                    var codeGenerationResult = codeGenerator.Generate(codeModelResult);

                    var fileGeneratorResults = fileGenerator.Generate(codeGenerationResult);
                    logger.LogFileGeneratorResults(fileGeneratorResults);
                    statistic.Update(fileGeneratorResults);
                }

                logger.LogProcessFileEnd(fileSpec);
            }

            logger.LogProcessEnd(statistic);

            return !logger.HasLoggedErrors;
        }
        
        sealed class Statistic {

            public int FileCount { get; private set; }
            public int FilesUpated { get; private set; }
            public int FilesSkiped { get; private set; }

            public void IncrementFileCount() {
                FileCount++;
            }

            public void Update(IImmutableList<FileGeneratorResult> fileResults) {
                foreach (var fileResult in fileResults) {
                    switch (fileResult.Action) {
                        case FileGeneratorAction.Skiped:
                            FilesSkiped++;
                            break;
                        case FileGeneratorAction.Updated:
                            FilesUpated++;
                            break;
                    }
                }
            }
        }
    }
}