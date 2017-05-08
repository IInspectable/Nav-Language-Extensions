#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class RemoveUnusedTaskDeclarationCodeFix : CodeFix {

        internal RemoveUnusedTaskDeclarationCodeFix(ITaskDeclarationSymbol taskDeclarationSymbol, CodeFixContext context)
            : base(context) {
            TaskDeclaration = taskDeclarationSymbol ?? throw new ArgumentNullException(nameof(taskDeclarationSymbol));
        }
        
        public override string Name              => "Remove Unused Task Declaration";
        public override CodeFixImpact Impact     => CodeFixImpact.None;
        public override TextExtent? ApplicableTo => null;
        public ITaskDeclarationSymbol TaskDeclaration { get; }
       
        internal bool CanApplyFix() {
            return TaskDeclaration.References.Count == 0 && 
                   TaskDeclaration.Syntax!=null && 
                   TaskDeclaration.Origin==TaskDeclarationOrigin.TaskDeclaration &&
                   TaskDeclaration.IsIncluded==false;
        }
        
        public IList<TextChange> GetTextChanges() {
            if(!CanApplyFix()) {
                throw new InvalidOperationException();
            }
            var textChanges = new List<TextChange?>();
            // ReSharper disable once PossibleNullReferenceException Siehe CanApplyFix
            textChanges.Add(TryRemove(TaskDeclaration.Syntax.GetFullExtent()));
            return textChanges.OfType<TextChange>().ToList();
        }     
    }
}