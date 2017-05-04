#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {
    
    sealed class RenameCodeFixProvider : SymbolVisitor<RenameCodeFix> {

        RenameCodeFixProvider(ISymbol originatingSymbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            OriginatingSymbol  = originatingSymbol;
            EditorSettings     = editorSettings;
            CodeGenerationUnit = codeGenerationUnit;
        }

        ISymbol OriginatingSymbol { get; }
        EditorSettings EditorSettings { get; }
        CodeGenerationUnit CodeGenerationUnit { get; }

        [CanBeNull]
        public static RenameCodeFix TryGetCodeFix(ISymbol symbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {
            var finder = new RenameCodeFixProvider(
                symbol             ?? throw new ArgumentNullException(nameof(symbol)),
                editorSettings     ?? throw new ArgumentNullException(nameof(editorSettings)),
                codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit))
            );

            var codeFix = finder.Visit(symbol);
            if (codeFix != null && codeFix.CanApplyFix()) {
                return codeFix;
            }

            return null;
        }

        public override RenameCodeFix VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
            // Wenn es bereits einen Alias gibt, dann funktioniert der Rename nur auf dem Alias-Symbol
            if (OriginatingSymbol == initNodeSymbol && initNodeSymbol.Alias != null) {
                return DefaultVisit(initNodeSymbol);
            }
            return new InitNodeRenameCodeFix(initNodeSymbol, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return new InitNodeRenameCodeFix(initNodeAliasSymbol.InitNode, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
            return new ExitNodeRenameCodeFix(exitNodeSymbol, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
            // Wenn es bereits einen Alias gibt, dann funktioniert der Rename nur auf dem Alias-Symbol
            if (OriginatingSymbol == taskNodeSymbol && taskNodeSymbol.Alias != null) {
                return DefaultVisit(taskNodeSymbol);
            }
            return new TaskNodeRenameCodeFix(taskNodeSymbol, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
            return new TaskNodeRenameCodeFix(taskNodeAliasSymbol.TaskNode, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
            return new ChoiceRenameCodeFix(choiceNodeSymbol, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
            return new DialogNodeRenameCodeFix(dialogNodeSymbol, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
            return new ViewNodeRenameCodeFix(viewNodeSymbol, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {
            return new TaskDeclarationRenameCodeFix(taskDeclarationSymbol, CodeGenerationUnit, EditorSettings);
        }

        public override RenameCodeFix VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
            if (taskDefinitionSymbol.AsTaskDeclaration == null) {
                return DefaultVisit(taskDefinitionSymbol);
            }
            return Visit(taskDefinitionSymbol.AsTaskDeclaration);
        }

        public override RenameCodeFix VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
            if (nodeReferenceSymbol.Declaration == null) {
                return DefaultVisit(nodeReferenceSymbol);
            }
            return Visit(nodeReferenceSymbol.Declaration);
        }
    }
}