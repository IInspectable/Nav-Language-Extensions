#region Using Directives

using System;
using System.Collections.Generic;

using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Logging;

#endregion

namespace Pharmatechnik.Nav.Language.Generator {

    public sealed partial class NavCodeGeneratorPipeline {

        public NavCodeGeneratorPipeline(GenerationOptions options,
                                        ILogger logger = null,
                                        ISyntaxProviderFactory syntaxProviderFactory = null) {

            Options         = options ?? throw new ArgumentNullException(nameof(options));
            Logger          = logger;
            ProviderFactory = syntaxProviderFactory ?? SyntaxProviderFactory.Default;
        }

        [NotNull]
        public GenerationOptions Options { get; }
        
        [NotNull]
        public ISyntaxProviderFactory ProviderFactory { get; }

        [CanBeNull]
        public ILogger Logger { get; }

        public bool Run(IEnumerable<FileSpec> fileSpecs) {

            using(var logger         = new LoggerAdapter(Logger))
            using(var syntaxProvider = ProviderFactory.CreateProvider())
            using(var codeGenerator  = new CodeGenerator(Options))
            using(var fileGenerator  = new FileGenerator(Options)) {

                var statistic = new Statistic();

                logger.LogProcessBegin();

                foreach (var fileSpec in fileSpecs) {

                    statistic.UpdatePerFile();

                    logger.LogProcessFileBegin(fileSpec);        
                
                    // 1. SyntaxTree
                    var syntax = syntaxProvider.FromFile(fileSpec.FilePath);
                    if(syntax == null) {
                        logger.LogError(String.Format(DiagnosticDescriptors.Semantic.Nav0004File0NotFound.MessageFormat, fileSpec));
                        continue;
                    }

                    // 2. Semantic Model
                    var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntax, syntaxProvider: syntaxProvider);

                    if (logger.LogErrors(syntax.SyntaxTree.Diagnostics) || logger.LogErrors(codeGenerationUnit.Diagnostics)) {
                        continue;
                    }

                    logger.LogWarnings(syntax.SyntaxTree.Diagnostics);
                    logger.LogWarnings(codeGenerationUnit.Diagnostics);

                    // 3. Generate Code
                    var codeGenerationResults = codeGenerator.Generate(codeGenerationUnit);
                    foreach (var codeGenerationResult in codeGenerationResults) {

                        // 4. Write Code into appropriate files
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
}