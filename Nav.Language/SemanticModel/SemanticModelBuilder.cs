#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {
    
    sealed class CodeGenerationUnitBuilder {

        readonly ISyntaxProvider _syntaxProvider;
        readonly List<Diagnostic> _diagnostics;
        readonly SymbolCollection<TaskDeclarationSymbol> _taskDeclarations;
        readonly SymbolCollection<TaskDefinitionSymbol> _taskDefinitions;
        readonly SymbolCollection<IncludeSymbol> _includes;
        readonly List<string>  _codeUsings;
        readonly List<ISymbol> _symbols;

        CodeGenerationUnitBuilder(ISyntaxProvider syntaxProvider) {
            _syntaxProvider   = syntaxProvider ?? SyntaxProvider.Default;
            _diagnostics      = new List<Diagnostic>();
            _taskDeclarations = new SymbolCollection<TaskDeclarationSymbol>();
            _taskDefinitions  = new SymbolCollection<TaskDefinitionSymbol>();
            _includes         = new SymbolCollection<IncludeSymbol>();
            _codeUsings       = new List<string>();
            _symbols          = new List<ISymbol>();
        }

        [NotNull]
        public static CodeGenerationUnit FromCodeGenerationUnitSyntax(CodeGenerationUnitSyntax syntax, CancellationToken cancellationToken, ISyntaxProvider syntaxProvider) {

            if (syntax == null) {
                throw new ArgumentNullException(nameof(syntax));
            }

            var builder = new CodeGenerationUnitBuilder(syntaxProvider);

            builder.Process(syntax, cancellationToken);

            var model=new CodeGenerationUnit(
                syntax, 
                builder._codeUsings, 
                builder._taskDeclarations, 
                builder._taskDefinitions,
                builder._includes,
                builder._symbols,
                builder._diagnostics);

            foreach(var taskDefinition in builder._taskDefinitions) {
                taskDefinition.FinalConstruct(model);
            }

            foreach (var taskDeclaration in builder._taskDeclarations.Where(td=>!td.IsIncluded)) {
                taskDeclaration.FinalConstruct(model);
            }

            return model;
        }

        void Process(CodeGenerationUnitSyntax syntax, CancellationToken cancellationToken) {
            ProcessNavLanguage(syntax, cancellationToken);
            ProcessCodeLanguage(syntax, cancellationToken);
            ProcessFinalSemanticErrors(syntax, cancellationToken);
        }

        void ProcessNavLanguage(CodeGenerationUnitSyntax syntax, CancellationToken cancellationToken) {

            cancellationToken.ThrowIfCancellationRequested();

            //====================
            // 1. TaskDeclarations 
            //====================
            var taskDeclarationResult = TaskDeclarationSymbolBuilder.FromCodeGenerationUnitSyntax(syntax, _syntaxProvider, cancellationToken);

            _diagnostics.AddRange(taskDeclarationResult.Diagnostics);
            _taskDeclarations.AddRange(taskDeclarationResult.TaskDeklarations);
            _includes.AddRange(taskDeclarationResult.Includes);

            cancellationToken.ThrowIfCancellationRequested();

            //====================
            // 2. TaskDefinitions
            //====================
            foreach (var taskDefinitionSyntax in syntax.DescendantNodes<TaskDefinitionSyntax>()) {
                ProcessTaskDefinitionSyntax(taskDefinitionSyntax, cancellationToken);
            }

            //====================
            // 3. Collect Symbols
            //====================
            // Nur Symbole von Taskdeklarationen der eigenen Datei, und auch nur solche, die aus "taskrefs task" entstanden sind
            _symbols.AddRange(_taskDeclarations.Where(td => !td.IsIncluded && 
                                                            td.Origin==TaskDeclarationOrigin.TaskDeclaration)
                                               .SelectMany(td => td.SymbolsAndSelf()));

            // Alle Symbole und deren "Kinder" der Taskdefinitionen
            _symbols.AddRange(_taskDefinitions.SelectMany(td => td.SymbolsAndSelf()));

            // Alle Includes (= taskref "filepath") hinzuf�gen
            _symbols.AddRange(_includes);            
        }
        
        void ProcessTaskDefinitionSyntax(TaskDefinitionSyntax taskDefinitionSyntax, CancellationToken cancellationToken) {
            
            cancellationToken.ThrowIfCancellationRequested();
            
            var taskDefinitionResult= TaskDefinitionSymbolBuilder.Build(taskDefinitionSyntax, _taskDeclarations);
            _diagnostics.AddRange(taskDefinitionResult.Diagnostics);

            var taskDefinition = taskDefinitionResult.TaskDefinition;
            if(taskDefinition == null) {
                return;
            }
            // Doppelte Eintr�ge �berspringen. Fehler existiert schon wegen der Taskdeclarations.
            if (!_taskDefinitions.Contains(taskDefinition.Name)) {
                _taskDefinitions.Add(taskDefinition);
            }
        }

        void ProcessCodeLanguage(CodeGenerationUnitSyntax syntax, CancellationToken cancellationToken) {
            foreach (var codeUsingDeclarationSyntax in syntax.DescendantNodes<CodeUsingDeclarationSyntax>()) {

                cancellationToken.ThrowIfCancellationRequested();

                ProcessCodeUsingDeclaration(codeUsingDeclarationSyntax);
            }
        }
        
        void ProcessCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax) {

            if (codeUsingDeclarationSyntax?.Namespace == null) {
                return;
            }

            var nsSyntax = codeUsingDeclarationSyntax.Namespace;
            var ns       = nsSyntax.ToString();

            if (_codeUsings.Contains(ns)) {
                _diagnostics.Add(new Diagnostic(
                    codeUsingDeclarationSyntax.GetLocation(),
                    DiagnosticDescriptors.DeadCode.Nav1002UsingDirective0AppearedPreviously,
                    ns));

            } else {
                _codeUsings.Add(ns);
            }
        }

        // ReSharper disable once UnusedParameter.Local
        void ProcessFinalSemanticErrors(CodeGenerationUnitSyntax syntax, CancellationToken cancellationToken) {

            // =====================
            // Unused Includes
            var unusedIncludes = _includes.Where(i => !i.TaskDeklarations.SelectMany(td => td.References).Any());
            foreach (var includeSymbol in unusedIncludes) {

                cancellationToken.ThrowIfCancellationRequested();

                _diagnostics.Add(new Diagnostic(
                    includeSymbol.Syntax.GetLocation(), 
                    DiagnosticDescriptors.DeadCode.Nav1003IncludeNotRequired));
            }
            
            // =====================
            // Unused Task Declarations
            var unusedTaskDeclarations = _taskDeclarations.Where(td => !td.IsIncluded && td.Origin == TaskDeclarationOrigin.TaskDeclaration && !td.References.Any());
            foreach (var taskDeclarationSymbol in unusedTaskDeclarations) {

                cancellationToken.ThrowIfCancellationRequested();

                // wenn m�glich markieren wir die ganze Syntax
                var location = taskDeclarationSymbol.Syntax?.GetLocation() ?? taskDeclarationSymbol.Location;

                _diagnostics.Add(new Diagnostic(
                    location, 
                    DiagnosticDescriptors.DeadCode.Nav1005TaskDeclaration0NotRequired,
                    taskDeclarationSymbol.Name));
            }
        }
    }
}