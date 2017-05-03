﻿#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.CodeFixes.Rename;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public abstract partial class CodeFix {

        public static IEnumerable<CodeFix> FindCodeFixes(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            return FindCodeFixes<CodeFix>(symbol, editorSettings, codeGenerationUnit);
        }

        public static IEnumerable<T> FindCodeFixes<T>(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) where T: CodeFix {
            return CodeFixFinder.Find(symbol, editorSettings, codeGenerationUnit).OfType<T>();
        }

        sealed class CodeFixFinder : SymbolVisitor<IEnumerable<CodeFix>> {

            CodeFixFinder(ISymbol originatingSymbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
                OriginatingSymbol  = originatingSymbol;
                EditorSettings     = editorSettings;
                CodeGenerationUnit = codeGenerationUnit;
            }

            ISymbol OriginatingSymbol { get; }
            EditorSettings EditorSettings { get; }
            CodeGenerationUnit CodeGenerationUnit { get; }

            public static IEnumerable<CodeFix> Find(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit){
                var finder = new CodeFixFinder(
                    symbol             ?? throw new ArgumentNullException(nameof(symbol)),
                    editorSettings     ?? throw new ArgumentNullException(nameof(editorSettings)),
                    codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit))
                );

                return finder.Visit(symbol).Where(cf => cf.CanApplyFix());
            }

            protected override IEnumerable<CodeFix> DefaultVisit(ISymbol symbol) {
                yield break;
            }

            public override IEnumerable<CodeFix> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
                if (OriginatingSymbol == initNodeSymbol || initNodeSymbol.Alias == null) {
                    return DefaultVisit(initNodeSymbol);
                }
                return Visit(initNodeSymbol.Alias);
            }

            public override IEnumerable<CodeFix> VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
                yield return new InitAliasSymbolRenameCodeFix(initNodeAliasSymbol, CodeGenerationUnit, EditorSettings);
            }

            public override IEnumerable<CodeFix> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
                if (OriginatingSymbol == taskNodeSymbol || taskNodeSymbol.Alias == null) {
                    return DefaultVisit(taskNodeSymbol);
                }

                return Visit(taskNodeSymbol.Alias);
            }

            public override IEnumerable<CodeFix> VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
                yield return new TaskNodeAliasSymbolRenameCodeFix(taskNodeAliasSymbol, CodeGenerationUnit, EditorSettings);
            }

            public override IEnumerable<CodeFix> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
                yield return new ChoiceSymbolRenameCodeFix(choiceNodeSymbol, CodeGenerationUnit, EditorSettings);
            }

            public override IEnumerable<CodeFix> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

                // Renames
                if (nodeReferenceSymbol.Declaration != null) {
                    foreach (var codefix in Visit(nodeReferenceSymbol.Declaration)) {
                        yield return codefix;
                    }
                }

                yield return new IntroduceChoiceCodeFix(nodeReferenceSymbol, CodeGenerationUnit, EditorSettings);

                // Add Missing Edge
                var taskNode = nodeReferenceSymbol.Declaration as ITaskNodeSymbol;
                if (taskNode != null) {
                    foreach (var missingExitConnectionPoint in taskNode.GetMissingExitTransitionConnectionPoints()) {
                        yield return new AddMissingExitTransitionCodeFix(nodeReferenceSymbol, missingExitConnectionPoint, CodeGenerationUnit, EditorSettings);   
                    }
                }                
            }
        }
    }
}