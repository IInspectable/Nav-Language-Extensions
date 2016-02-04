#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed class TaskDeclarationResult {
        public TaskDeclarationResult(IReadOnlyList<Diagnostic> diagnostics, 
                                     SymbolCollection<TaskDeclarationSymbol> taskDeklarations, 
                                     SymbolCollection<IncludeSymbol> includes) {

            Diagnostics      = diagnostics      ?? new List<Diagnostic>();
            TaskDeklarations = taskDeklarations ?? new SymbolCollection<TaskDeclarationSymbol>();
            Includes         = includes         ?? new SymbolCollection<IncludeSymbol>();
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public SymbolCollection<TaskDeclarationSymbol> TaskDeklarations { get; }
        public SymbolCollection<IncludeSymbol> Includes { get; }
    }

    sealed class TaskDeclarationSymbolBuilder {

        readonly bool                                    _processAsIncludedFile;
        readonly List<Diagnostic>                        _diagnostics;
        readonly SymbolCollection<TaskDeclarationSymbol> _taskDeklarations;
        readonly SymbolCollection<IncludeSymbol>         _includes;

        public TaskDeclarationSymbolBuilder(bool processAsIncludedFile) {
            _diagnostics           = new List<Diagnostic>();
            _processAsIncludedFile = processAsIncludedFile;
            _taskDeklarations      = new SymbolCollection<TaskDeclarationSymbol>();
            _includes              = new SymbolCollection<IncludeSymbol>();
        }
      
        public static TaskDeclarationResult FromCompilationUnit(CompilationUnitSyntax syntax, CancellationToken cancellationToken) {
            return FromCompilationUnit(syntax, false, cancellationToken);
        }

        static TaskDeclarationResult FromCompilationUnit(CompilationUnitSyntax syntax, bool processAsIncludedFile, CancellationToken cancellationToken) {
            var builder = new TaskDeclarationSymbolBuilder(processAsIncludedFile);
            builder.ProcessCompilationUnit(syntax, cancellationToken);

            return new TaskDeclarationResult(builder._diagnostics, builder._taskDeklarations, builder._includes);
        }

        void ProcessCompilationUnit(CompilationUnitSyntax syntax, CancellationToken cancellationToken) {

            if (!_processAsIncludedFile) {
                foreach(var includeDirectiveSyntax in syntax.DescendantNodes().OfType<IncludeDirectiveSyntax>()) {
                    cancellationToken.ThrowIfCancellationRequested();
                    ProcessIncludeDirective(includeDirectiveSyntax, cancellationToken);
                }
                foreach (var taskDeclarationSyntax in syntax.DescendantNodes().OfType<TaskDeclarationSyntax>()) {
                    cancellationToken.ThrowIfCancellationRequested();
                    ProcessTaskDeclaration(taskDeclarationSyntax);
                }
            }
            foreach (var taskDefinitionSyntax in syntax.DescendantNodes().OfType<TaskDefinitionSyntax>()) {
                cancellationToken.ThrowIfCancellationRequested();
                ProcessTaskDefinition(taskDefinitionSyntax);
            }
        }

        void ProcessIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax, CancellationToken cancellationToken) {
           
            var location = includeDirectiveSyntax.StringLiteral.GetLocation();

            try {

                var filePath = includeDirectiveSyntax.StringLiteral.ToString().Trim('"');
                if(!Path.IsPathRooted(filePath)) {

                    var directory = includeDirectiveSyntax.SyntaxTree.FileInfo?.Directory;
                    if (directory == null || !directory.Exists) {

                        _diagnostics.Add(new Diagnostic(
                            location,
                            DiagnosticDescriptors.Semantic.Nav0003SourceFileNeedsToBeSavedBeforeIncludeDirectiveCanBeProcessed));

                        return;
                    }

                    filePath = Path.Combine(directory.FullName, filePath);
                }

                if(!File.Exists(filePath)) {
                    _diagnostics.Add(new Diagnostic(
                        location, 
                        DiagnosticDescriptors.Semantic.Nav0004File0NotFound, 
                        filePath) );
                    return;
                } else {
                    // FileInfo löst relative Pfadangaben auf...
                    var fileInfo = new FileInfo(filePath);
                    filePath = fileInfo.FullName;
                }

                // nav File inkludiert sich selbst
                if (String.Equals(includeDirectiveSyntax.SyntaxTree.FileInfo?.FullName, filePath, StringComparison.OrdinalIgnoreCase)) {

                    _diagnostics.Add(new Diagnostic(
                        location,
                        DiagnosticDescriptors.DeadCode.Nav1006SelfReferencingIncludeNotRequired));

                    return;
                }
                
                var includeFileSyntax = SyntaxTree.FromFile(filePath, cancellationToken);
                var fileLocation      = new Location(filePath);
                var result            = FromCompilationUnit(includeFileSyntax.GetRoot() as CompilationUnitSyntax, processAsIncludedFile: true, cancellationToken: cancellationToken);
                var diagnostics       = includeFileSyntax.Diagnostics.Union(result.Diagnostics).ToList();
                var include           = new IncludeSymbol(filePath, location, fileLocation, includeDirectiveSyntax, diagnostics, result.TaskDeklarations);

                AddInclude(include);
                
            } catch(OperationCanceledException) {
                throw;
            } catch(Exception ex) {
                _diagnostics.Add(new Diagnostic(location, DiagnosticDescriptors.NewInternalError(ex)));
            }
        }

        void AddInclude(IncludeSymbol include) {

            var existing = _includes.TryFindSymbol(include);
            if (existing!=null) {
                
                _diagnostics.Add(new Diagnostic(
                    include.Location,
                    DiagnosticDescriptors.DeadCode.Nav1001IncludeDirectiveForFile0AppearedPreviously, 
                    include.FileName));

            } else {
                _includes.Add(include);

                if(include.Diagnostics.HasErrors()) {
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
                if(location != null) {

                    var taskDeclaration = new TaskDeclarationSymbol(identifier.ToString(), location, TaskDeclarationOrigin.TaskDeclaration,  _processAsIncludedFile);

                    AddConnectionPoints(taskDeclaration, taskDeclarationSyntax.ConnectionPoints);
                    AddTaskDeclaration(taskDeclaration);
                }                
            }
        }

        void ProcessTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) {

            if(null != taskDefinitionSyntax && !taskDefinitionSyntax.Identifier.IsMissing) {

                var identifier = taskDefinitionSyntax.Identifier;
                var location   = identifier.GetLocation();
                if(location != null) {
                    var taskDeclaration = new TaskDeclarationSymbol(identifier.ToString(), location, TaskDeclarationOrigin.TaskDefinition, _processAsIncludedFile);
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
                    var init     = new InitConnectionPointSymbol(name, location, initNodeSyntax);

                    AddConnectionPoint(taskDeclaration, init);
                }

                foreach (var exitNodeSyntax in connectionPoints.OfType<ExitNodeDeclarationSyntax>()) {

                    var identifier = exitNodeSyntax.Identifier.IsMissing ? exitNodeSyntax.ExitKeyword: exitNodeSyntax.Identifier;

                    var location = identifier.GetLocation();
                    var name     = identifier.ToString();
                    var exit     = new ExitConnectionPointSymbol(name, location, exitNodeSyntax);

                    AddConnectionPoint(taskDeclaration, exit);
                }

                foreach (var endNodeSyntax in connectionPoints.OfType<EndNodeDeclarationSyntax>()) {
                    var identifier = endNodeSyntax.EndKeyword;

                    var location = identifier.GetLocation();
                    var name     = identifier.ToString();
                    var end      = new EndConnectionPointSymbol(name, location, endNodeSyntax);

                    AddConnectionPoint(taskDeclaration, end);
                }
            }
        }

        void AddTaskDeclaration(TaskDeclarationSymbol taskDeclaration) {
            if (_taskDeklarations.Contains(taskDeclaration.Name)) {
                var existing = _taskDeklarations[taskDeclaration.Name];
                _diagnostics.Add(new Diagnostic(
                    existing.Location, 
                    DiagnosticDescriptors.Semantic.Nav0020TaskWithName0AlreadyDeclared,
                    existing.Name));

                _diagnostics.Add(new Diagnostic(
                    taskDeclaration.Location,
                    DiagnosticDescriptors.Semantic.Nav0020TaskWithName0AlreadyDeclared,
                    taskDeclaration.Name));

            } else {
                if (!CSharp.IsValidIdentifier(taskDeclaration.Name)) {
                    _diagnostics.Add(new Diagnostic(
                        taskDeclaration.Location,
                        DiagnosticDescriptors.Semantic.Nav2000IdentifierExpected));
                }
                _taskDeklarations.Add(taskDeclaration);
            }
        }

        void AddConnectionPoint(TaskDeclarationSymbol taskDeclaration, ConnectionPointSymbol connectionPoint) {

            if (taskDeclaration.ConnectionPoints.Contains(connectionPoint.Name)) {
                var existing = taskDeclaration.ConnectionPoints[connectionPoint.Name];
                _diagnostics.Add(new Diagnostic(
                    existing.Location,
                    DiagnosticDescriptors.Semantic.Nav0021ConnectionPointWithName0AlreadyDeclared,
                    existing.Name));

                _diagnostics.Add(new Diagnostic(
                    connectionPoint.Location,
                    DiagnosticDescriptors.Semantic.Nav0021ConnectionPointWithName0AlreadyDeclared,
                    connectionPoint.Name));

            } else {
                taskDeclaration.ConnectionPoints.Add(connectionPoint);
            }
        }
    }
}