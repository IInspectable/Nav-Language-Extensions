#region Using Directives

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public sealed partial class NavCodeGeneratorPipeline {
        
        [CanBeNull]
        readonly ILogger _logger;
        [NotNull]
        readonly ISyntaxProviderFactory _syntaxProviderFactory;

        public NavCodeGeneratorPipeline(GenerationOptions options, 
                                        ILogger logger, 
                                        ISyntaxProviderFactory syntaxProviderFactory = null) {
            
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _syntaxProviderFactory = syntaxProviderFactory ?? SyntaxProviderFactory.Default;
        }

        [NotNull]
        public GenerationOptions Options { get; }
        
        public bool Run(IEnumerable<FileSpec> fileSpecs) {

            using(var logger         = new LoggerAdapter(_logger))
            using(var syntaxProvider = _syntaxProviderFactory.CreateProvider())
            using(var modelGenerator = new CodeModelGenerator(Options))
            using(var codeGenerator  = new CodeGenerator(Options))
            using(var fileGenerator  = new FileGenerator(Options)) {

                var statistic = new Statistic();

                logger.LogProcessBegin();

                foreach (var fileSpec in fileSpecs) {

                    statistic.UpdatePerFile();

                    logger.LogProcessFileBegin(fileSpec);        
                
                    // 1. SyntaxTree
                    var syntaxTree = syntaxProvider.FromFile(fileSpec.FilePath);
                    if(syntaxTree == null) {
                        logger.LogError(String.Format(DiagnosticDescriptors.Semantic.Nav0004File0NotFound.MessageFormat, fileSpec));
                        continue;
                    }
                    // 2. Semantic Model
                    var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax((CodeGenerationUnitSyntax)syntaxTree.GetRoot(), syntaxProvider: syntaxProvider);

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
}