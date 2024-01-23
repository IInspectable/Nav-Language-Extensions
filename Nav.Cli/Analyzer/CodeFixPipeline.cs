using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.CodeFixes.StyleFix;
using Pharmatechnik.Nav.Language.Generator;
using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language.Analyzer;

class CodeFixPipeline {

    readonly ILogger                _logger;
    readonly ISyntaxProviderFactory _syntaxProviderFactory;
    readonly ISemanticModelProvider _semanticModelProvider;

    public CodeFixPipeline(ILogger logger, ISyntaxProviderFactory syntaxProviderFactory = null) {
        _logger                = logger;
        _syntaxProviderFactory = syntaxProviderFactory ?? SyntaxProviderFactory.Default;
        _semanticModelProvider = SemanticModelProvider.Default;
    }

    public void Run(IEnumerable<FileSpec> files, Func<string, bool> checkout, Func<CodeFixContext, CancellationToken, IEnumerable<StyleCodeFix>> suggestCodeFixes) {
        var settings = new TextEditorSettings(4, Environment.NewLine);

        using var syntaxProvider = _syntaxProviderFactory.CreateProvider();
        foreach (var file in files) {
            // 1. SyntaxTree
            var syntax = syntaxProvider.GetSyntax(file.FilePath);

            if (syntax == null) {
                _logger?.LogError(String.Format(DiagnosticDescriptors.Semantic.Nav0004File0NotFound.MessageFormat, file));
                continue;
            }

            var model = _semanticModelProvider.GetSemanticModel(syntax);

            var codeFixcontext = new CodeFixContext(syntax.Extent, model, settings);

            var fixes = suggestCodeFixes(codeFixcontext, default).ToList();

            if (!fixes.Any()) {
                continue;
            }

            // _logger?.LogError(file.FilePath);
            if (!checkout(file.FilePath)) {
                _logger?.LogError(file.FilePath);
                continue;
            }

            var changes   = fixes.SelectMany(fix => fix.GetTextChanges());
            var newString = ApplyChanges(syntax.SyntaxTree.SourceText.Text, changes);

            File.WriteAllText(file.FilePath, newString, Encoding.UTF8);

            _logger?.LogInfo($"Codefix applied for {file.FilePath}");

        }
    }

    string ApplyChanges(string text, IEnumerable<TextChange> textChanges) {
        var writer = new TextChangeWriter();
        return writer.ApplyTextChanges(text, textChanges);
    }

}