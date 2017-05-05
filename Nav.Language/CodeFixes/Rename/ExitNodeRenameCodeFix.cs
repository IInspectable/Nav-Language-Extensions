#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class ExitNodeRenameCodeFix : RenameNodeCodeFix<IExitNodeSymbol> {
        
        internal ExitNodeRenameCodeFix(IExitNodeSymbol exitNodeSymbol, CodeFixContext context) 
            : base(exitNodeSymbol, context) {
        }

        public override string Name          => "Rename Exit";
        public override CodeFixImpact Impact => CodeFixImpact.High;
        IExitNodeSymbol ExitNode             => Symbol;
        
        public override IEnumerable<TextChange> GetTextChanges(string newName) {

            newName = newName?.Trim()??String.Empty;

            var validationMessage = ValidateSymbolName(newName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newName));
            }
            
            var textChanges = new List<TextChange?>();
            // Das Exit selbst
            textChanges.Add(TryRename(ExitNode, newName));

            // Die Exit-Referenzen auf der "rechten Seite"
            foreach (var transition in ExitNode.Incomings) {
                var textChange = TryRenameTarget(transition, newName);
                textChanges.Add(textChange);
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}