#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public sealed partial class NavCodeGeneratorPipeline {

        readonly IGeneratorLogger _logger;

        public NavCodeGeneratorPipeline(GenerationOptions options, IGeneratorLogger logger) {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        [NotNull]
        public GenerationOptions Options { get; }

        public bool Run(IEnumerable<FileSpec> fileSpecs) {

            var logger         = new LoggerWrapper(_logger);
            var modelGenerator = new CodeModelGenerator(Options);
            var codeGenerator  = new CodeGenerator(Options);
            var fileGenerator  = new FileGenerator(Options);

            foreach (var fileSpec in fileSpecs) {

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

                    fileGenerator.Generate(codeGenerationResult);
                }
            }

            return !logger.HasLoggedErrors;
        }
    }
}