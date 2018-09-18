#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed class TaskDeclarationSymbolBuilder {

        readonly CodeGenerationUnitSyntax                _codeGenerationUnitSyntax;
        readonly bool                                    _processAsIncludedFile;
        readonly ISyntaxProvider                         _syntaxProvider;
        readonly List<Diagnostic>                        _diagnostics;
        readonly SymbolCollection<TaskDeclarationSymbol> _taskDeklarations;
        readonly SymbolCollection<IncludeSymbol>         _includes;

        TaskDeclarationSymbolBuilder(CodeGenerationUnitSyntax codeGenerationUnitSyntax,
                                     bool processAsIncludedFile,
                                     ISyntaxProvider syntaxProvider) {
            _codeGenerationUnitSyntax = codeGenerationUnitSyntax;
            _diagnostics              = new List<Diagnostic>();
            _processAsIncludedFile    = processAsIncludedFile;
            _syntaxProvider           = syntaxProvider ?? SyntaxProvider.Default;
            _taskDeklarations         = new SymbolCollection<TaskDeclarationSymbol>();
            _includes                 = new SymbolCollection<IncludeSymbol>();
        }

        public static (
            IReadOnlyList<Diagnostic> Diagnostics,
            SymbolCollection<TaskDeclarationSymbol> TaskDeklarations,
            SymbolCollection<IncludeSymbol> Includes)
            FromCodeGenerationUnitSyntax(CodeGenerationUnitSyntax syntax, ISyntaxProvider syntaxProvider, CancellationToken cancellationToken) {

            return FromCodeGenerationUnitSyntax(syntax, false, syntaxProvider, cancellationToken);
        }

        static (
            IReadOnlyList<Diagnostic> Diagnostics,
            SymbolCollection<TaskDeclarationSymbol> TaskDeklarations,
            SymbolCollection<IncludeSymbol> Includes)
            FromCodeGenerationUnitSyntax(CodeGenerationUnitSyntax syntax, bool processAsIncludedFile, ISyntaxProvider syntaxProvider, CancellationToken cancellationToken) {

            var builder = new TaskDeclarationSymbolBuilder(syntax, processAsIncludedFile, syntaxProvider);
            builder.ProcessCodeGenerationUnitSyntax(syntax, cancellationToken);

            return (Diagnostics: builder._diagnostics,
                TaskDeklarations: builder._taskDeklarations,
                Includes: builder._includes);
        }

        void ProcessCodeGenerationUnitSyntax(CodeGenerationUnitSyntax syntax, CancellationToken cancellationToken) {

            if (!_processAsIncludedFile) {

                foreach (var includeDirectiveSyntax in syntax.DescendantNodes<IncludeDirectiveSyntax>()) {
                    cancellationToken.ThrowIfCancellationRequested();
                    ProcessIncludeDirective(includeDirectiveSyntax, cancellationToken);
                }

                foreach (var taskDeclarationSyntax in syntax.DescendantNodes<TaskDeclarationSyntax>()) {
                    cancellationToken.ThrowIfCancellationRequested();
                    ProcessTaskDeclaration(taskDeclarationSyntax);
                }
            }

            foreach (var taskDefinitionSyntax in syntax.DescendantNodes<TaskDefinitionSyntax>()) {
                cancellationToken.ThrowIfCancellationRequested();
                ProcessTaskDefinition(taskDefinitionSyntax);
            }
        }

        void ProcessIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax, CancellationToken cancellationToken) {

            var location = includeDirectiveSyntax.StringLiteral.GetLocation();

            try {

                var filePath = includeDirectiveSyntax.StringLiteral.ToString().Trim('"').Trim();
                if (!Path.IsPathRooted(filePath)) {

                    var directory = includeDirectiveSyntax.SyntaxTree.SourceText.FileInfo?.Directory;
                    if (directory == null) {

                        _diagnostics.Add(new Diagnostic(
                                             location,
                                             DiagnosticDescriptors.Semantic.Nav0003SourceFileNeedsToBeSavedBeforeIncludeDirectiveCanBeProcessed));

                        return;
                    }

                    filePath = Path.Combine(directory.FullName, filePath);
                }

                // L�st relative Pfadangaben auf...
                filePath = Path.GetFullPath(filePath);

                // nav File inkludiert sich selbst
                if (String.Equals(includeDirectiveSyntax.SyntaxTree.SourceText.FileInfo?.FullName, filePath, StringComparison.OrdinalIgnoreCase)) {

                    _diagnostics.Add(new Diagnostic(
                                         location,
                                         DiagnosticDescriptors.DeadCode.Nav1006SelfReferencingIncludeNotRequired));

                    return;
                }

                var includeFileSyntax = _syntaxProvider.GetSyntax(filePath, cancellationToken);
                if (includeFileSyntax == null) {
                    _diagnostics.Add(new Diagnostic(
                                         location,
                                         DiagnosticDescriptors.Semantic.Nav0004File0NotFound,
                                         filePath));
                    return;

                }

                var fileLocation = new Location(filePath);
                var result       = FromCodeGenerationUnitSyntax(includeFileSyntax, processAsIncludedFile: true, syntaxProvider: _syntaxProvider, cancellationToken: cancellationToken);
                var diagnostics  = includeFileSyntax.SyntaxTree.Diagnostics.Union(result.Diagnostics).ToList();
                var include      = new IncludeSymbol(filePath, location, fileLocation, includeDirectiveSyntax, diagnostics, result.TaskDeklarations);

                AddInclude(include);

            } catch (OperationCanceledException) {
                throw;
            } catch (Exception ex) {
                _diagnostics.Add(new Diagnostic(location, DiagnosticDescriptors.NewInternalError(ex)));
            }
        }

        void AddInclude(IncludeSymbol include) {

            var existing = _includes.TryFindSymbol(include);
            if (existing != null) {

                _diagnostics.Add(new Diagnostic(
                                     include.Location,
                                     DiagnosticDescriptors.DeadCode.Nav1001IncludeDirectiveForFile0AppearedPreviously,
                                     Path.GetFileName(include.FileName)));

            } else {
                _includes.Add(include);

                if (include.Diagnostics.HasErrors()) {
                    _diagnostics.Add(new Diagnostic(
                                         include.Location,
                                         DiagnosticDescriptors.Semantic.Nav0005IncludeFile0HasSomeErrors,
                                         include.FileName));
                }

                foreach (var decl in include.TaskDeklarations) {
                    AddTaskDeclaration(decl);
                }
            }
        }

        void ProcessTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax) {

            if (null != taskDeclarationSyntax && !taskDeclarationSyntax.Identifier.IsMissing) {

                var identifier = taskDeclarationSyntax.Identifier;
                var location   = identifier.GetLocation();
                if (location != null) {

                    var syntax = _processAsIncludedFile ? null : taskDeclarationSyntax;

                    var taskDeclaration = new TaskDeclarationSymbol(
                        name: identifier.ToString(),
                        location: location,
                        origin: TaskDeclarationOrigin.TaskDeclaration,
                        isIncluded: _processAsIncludedFile,
                        codeTaskResult: CodeParameter.FromResultDeclaration(taskDeclarationSyntax.CodeResultDeclaration),
                        syntax: syntax,
                        codeNamespace: taskDeclarationSyntax.CodeNamespaceDeclaration?.Namespace?.Text,
                        codeNotImplemented: taskDeclarationSyntax.CodeNotImplementedDeclaration != null);

                    AddConnectionPoints(taskDeclaration, taskDeclarationSyntax.ConnectionPoints);
                    AddTaskDeclaration(taskDeclaration);
                }
            }
        }

        void ProcessTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) {

            if (null != taskDefinitionSyntax && !taskDefinitionSyntax.Identifier.IsMissing) {

                var identifier = taskDefinitionSyntax.Identifier;
                var location   = identifier.GetLocation();
                if (location != null) {

                    // Speicher checken...
                    // var syntax = _processAsIncludedFile ? null : taskDefinitionSyntax;
                    var syntax = taskDefinitionSyntax;

                    var taskDeclaration = new TaskDeclarationSymbol(
                        name: identifier.ToString(),
                        location: location,
                        origin: TaskDeclarationOrigin.TaskDefinition,
                        isIncluded: _processAsIncludedFile,
                        codeTaskResult: CodeParameter.FromResultDeclaration(taskDefinitionSyntax.CodeResultDeclaration),
                        syntax: syntax,
                        codeNamespace: _codeGenerationUnitSyntax?.CodeNamespace?.Namespace?.Text,
                        codeNotImplemented: false
                    );

                    AddConnectionPoints(taskDeclaration, taskDefinitionSyntax.NodeDeclarationBlock?.ConnectionPoints().ToList());
                    AddTaskDeclaration(taskDeclaration);
                }
            }
        }

        void AddConnectionPoints(TaskDeclarationSymbol taskDeclaration, IReadOnlyList<ConnectionPointNodeSyntax> connectionPoints) {

            if (connectionPoints != null) {

                foreach (var initNodeSyntax in connectionPoints.OfType<InitNodeDeclarationSyntax>()) {

                    var identifier = initNodeSyntax.Identifier.IsMissing ? initNodeSyntax.InitKeyword : initNodeSyntax.Identifier;

                    var location = identifier.GetLocation();
                    var name     = identifier.ToString();
                    var init     = new InitConnectionPointSymbol(name, location, initNodeSyntax, taskDeclaration);

                    AddConnectionPoint(taskDeclaration, init);
                }

                foreach (var exitNodeSyntax in connectionPoints.OfType<ExitNodeDeclarationSyntax>()) {

                    var identifier = exitNodeSyntax.Identifier.IsMissing ? exitNodeSyntax.ExitKeyword : exitNodeSyntax.Identifier;

                    var location = identifier.GetLocation();
                    var name     = identifier.ToString();
                    var exit     = new ExitConnectionPointSymbol(name, location, exitNodeSyntax, taskDeclaration);

                    AddConnectionPoint(taskDeclaration, exit);
                }

                foreach (var endNodeSyntax in connectionPoints.OfType<EndNodeDeclarationSyntax>()) {
                    var identifier = endNodeSyntax.EndKeyword;

                    var location = identifier.GetLocation();
                    var name     = identifier.ToString();
                    var end      = new EndConnectionPointSymbol(name, location, endNodeSyntax, taskDeclaration);

                    AddConnectionPoint(taskDeclaration, end);
                }
            }
        }

        void AddTaskDeclaration(TaskDeclarationSymbol taskDeclaration) {
            if (_taskDeklarations.Contains(taskDeclaration.Name)) {

                var existing = _taskDeklarations[taskDeclaration.Name];

                _diagnostics.Add(new Diagnostic(
                                     location: taskDeclaration.Location,
                                     additionalLocation: existing.Location,
                                     descriptor: DiagnosticDescriptors.Semantic.Nav0020TaskWithName0AlreadyDeclared,
                                     messageArgs: taskDeclaration.Name));

            } else {

                _taskDeklarations.Add(taskDeclaration);
            }
        }

        void AddConnectionPoint(TaskDeclarationSymbol taskDeclaration, ConnectionPointSymbol connectionPoint) {

            if (taskDeclaration.ConnectionPoints.Contains(connectionPoint.Name)) {

                var existing = taskDeclaration.ConnectionPoints[connectionPoint.Name];

                _diagnostics.Add(new Diagnostic(
                                     location: connectionPoint.Location,
                                     additionalLocation: existing.Location,
                                     descriptor: DiagnosticDescriptors.Semantic.Nav0021ConnectionPointWithName0AlreadyDeclared,
                                     messageArgs: connectionPoint.Name));

            } else {
                taskDeclaration.ConnectionPoints.Add(connectionPoint);
            }
        }

    }

}