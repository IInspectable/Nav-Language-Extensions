#region Using Directives

using System;
using System.IO;
using System.Collections.Generic;

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

                statistic.UpdatePerFile();

                logger.LogProcessFileBegin(fileSpec);

                if (!File.Exists(fileSpec.FilePath)) {
                    logger.LogError(String.Format(DiagnosticDescriptors.Semantic.Nav0004File0NotFound.MessageFormat, fileSpec));
                    continue;
                }
                
                // 1. SyntaxTree
                var syntaxTree = SyntaxTree.FromFile(fileSpec.FilePath);
                // 2. Semantic Model
                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax((CodeGenerationUnitSyntax)syntaxTree.GetRoot());

                if (logger.LogErrors(fileSpec, syntaxTree.Diagnostics) || logger.LogErrors(fileSpec, codeGenerationUnit.Diagnostics)) {
                    continue;
                }

                logger.LogWarnings(fileSpec, syntaxTree.Diagnostics);
                logger.LogWarnings(fileSpec, codeGenerationUnit.Diagnostics);

                // 3. Code Models
                var codeModelResults = modelGenerator.Generate(codeGenerationUnit);
                foreach (var codeModelResult in codeModelResults) {
                    
                    // 4. Code Generation
                    var codeGenerationResult = codeGenerator.Generate(codeModelResult);
                    
                    // 5. Write appropriate files
                    var fileGeneratorResults = fileGenerator.Generate(codeGenerationResult);

                    logger.LogFileGeneratorResults(fileGeneratorResults);

                    statistic.UpdatePerTask(fileGeneratorResults);
                }

                logger.LogProcessFileEnd(fileSpec);
            }

            logger.LogProcessEnd(statistic);

            return !logger.HasLoggedErrors;
        }
    }
}