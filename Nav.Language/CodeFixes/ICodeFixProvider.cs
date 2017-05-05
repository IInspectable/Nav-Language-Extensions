#region Using Directives

using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    //TODO ICodeFixProvider einführen, mit TextExtent, CodeGenerationUnit, EditorSettings und CancellationToken als Parameter

    public interface ICodeFixProvider {
        IEnumerable<CodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken=default(CancellationToken));
    }
    
    public interface ICodeFixProvider<out T> : ICodeFixProvider where T : CodeFix {
        new IEnumerable<T> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken = default(CancellationToken));
    }

    abstract class CodeFixProvider<T>: ICodeFixProvider<T> where T: CodeFix {

        public abstract IEnumerable<T> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken = new CancellationToken());
        
        IEnumerable<CodeFix> ICodeFixProvider.SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {
            return SuggestCodeFixes(context, cancellationToken);
        }      
    }
}