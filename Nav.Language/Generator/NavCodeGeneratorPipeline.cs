#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.Generator {

    public sealed partial class NavCodeGeneratorPipeline {

        NavCodeGeneratorPipeline(GenerationOptions options,
                                 ILogger logger,
                                 ISyntaxProviderFactory syntaxProviderFactory,
                                 IPathProviderFactory pathProviderFactory,
                                 ICodeGeneratorProvider codeGeneratorProvider,
                                 IFileGeneratorProvider fileGeneratorProvider) {

            Logger                = logger;
            Options               = options               ?? GenerationOptions.Default;
            SyntaxProviderFactory = syntaxProviderFactory ?? Language.SyntaxProviderFactory.Default;
            PathProviderFactory   = pathProviderFactory   ?? Language.PathProviderFactory.Default;
            CodeGeneratorProvider = codeGeneratorProvider ?? CodeGen.CodeGeneratorProvider.Default;
            FileGeneratorProvider = fileGeneratorProvider ?? CodeGen.FileGeneratorProvider.Default;
        }

        public static NavCodeGeneratorPipeline CreateDefault() => Create();

        public static NavCodeGeneratorPipeline Create(GenerationOptions options = null,
                                                      ILogger logger = null,
                                                      ISyntaxProviderFactory syntaxProviderFactory = null,
                                                      IPathProviderFactory pathProviderFactory = null,
                                                      ICodeGeneratorProvider codeGeneratorProvider = null,
                                                      IFileGeneratorProvider fileGeneratorProvider = null)
            => new NavCodeGeneratorPipeline(options              : options,
                                            logger               : logger,
                                            syntaxProviderFactory: syntaxProviderFactory,
                                            pathProviderFactory  : pathProviderFactory,
                                            codeGeneratorProvider: codeGeneratorProvider,
                                            fileGeneratorProvider: fileGeneratorProvider);

        [NotNull]
        public GenerationOptions Options { get; }

        [NotNull]
        public ISyntaxProviderFactory SyntaxProviderFactory { get; }

        [NotNull]
        public IPathProviderFactory PathProviderFactory { get; }

        [NotNull]
        public ICodeGeneratorProvider CodeGeneratorProvider { get; }

        [NotNull]
        public IFileGeneratorProvider FileGeneratorProvider { get; }

        [CanBeNull]
        public ILogger Logger { get; }

        public bool Run(IEnumerable<FileSpec> fileSpecs) {

            using (var logger = new LoggerAdapter(Logger))
            using (var syntaxProvider = SyntaxProviderFactory.CreateProvider())
            using (var codeGenerator = CodeGeneratorProvider.Create(Options, PathProviderFactory))
            using (var fileGenerator = FileGeneratorProvider.Create(Options)) {

                var statistic = new Statistic();

                logger.LogProcessBegin();

                foreach (var fileSpec in fileSpecs) {

                    statistic.UpdatePerFile();

                    logger.LogProcessFileBegin(fileSpec);

                    // 1. SyntaxTree
                    var syntax = syntaxProvider.FromFile(fileSpec.FilePath);
                    if (syntax == null) {
                        logger.LogError(String.Format(DiagnosticDescriptors.Semantic.Nav0004File0NotFound.MessageFormat, fileSpec));
                        continue;
                    }

                    // 2. Semantic Model
                    var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntax, syntaxProvider: syntaxProvider);

                    if (logger.LogErrors(syntax.SyntaxTree.Diagnostics)  ||
                        logger.LogErrors(codeGenerationUnit.Diagnostics) ||
                        logger.LogErrors(codeGenerationUnit.Includes.SelectMany(include => include.Diagnostics))) {
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