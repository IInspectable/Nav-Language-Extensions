#region Using Directives

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Pharmatechnik.Nav.Language.Generator;

#endregion

namespace Pharmatechnik.Nav.Language.Analyzer {

    class SyntaxAnalyzerPipeline  {

        readonly ILogger _logger;
        readonly ISyntaxProviderFactory _syntaxProviderFactory;

        public SyntaxAnalyzerPipeline(ILogger logger, ISyntaxProviderFactory syntaxProviderFactory = null) {
            _logger = logger;
            _syntaxProviderFactory = syntaxProviderFactory ?? SyntaxProviderFactory.Default;
        }

        public void Run<T>(IEnumerable<FileSpec> files, T analyzer) where T : SyntaxNodeAnalyzer {

            analyzer.Logger = _logger;
         //   using (var logger = new LoggerAdapter(_logger))
            using (var syntaxProvider = _syntaxProviderFactory.CreateProvider()) {
                foreach (var file in files) {
                    analyzer.CurrentFile = file;
                    // 1. SyntaxTree
                    var syntax = syntaxProvider.FromFile(file.FilePath);
                    if (syntax == null) {
                         _logger?.LogError(String.Format(DiagnosticDescriptors.Semantic.Nav0004File0NotFound.MessageFormat, file));
                        continue;
                    }

                    analyzer.Walk(syntax);
                }
            }
        }
    }

    public abstract class SyntaxNodeAnalyzer : SyntaxNodeWalker  {
        public FileSpec CurrentFile { get; set; }
        public ILogger Logger{ get; set; }
    }

    public class CodeNotImplementedAnalyzer: SyntaxNodeAnalyzer {

        public CodeNotImplementedAnalyzer(string pattern) {
            pattern = String.IsNullOrEmpty(pattern) ? ".*": $".*{pattern}.*";

            Pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Result  = 0;
        }

        public int Result { get; set; }
        public Regex Pattern { get; }

        public override bool DefaultWalk(SyntaxNode node) {

            if (Pattern.IsMatch(node.GetType().Name)) {
                Logger.LogInfo($"'{CurrentFile.Identity}");
                Result += 1;
            }

            return base.DefaultWalk(node);
        }     
    }
}