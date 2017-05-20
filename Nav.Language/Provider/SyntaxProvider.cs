#region Using Directives

using System.Threading;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class SyntaxProvider : ISyntaxProvider {
        
        public static readonly ISyntaxProvider Default = new SyntaxProvider();

        public virtual SyntaxTree FromFile(string filePath, CancellationToken cancellationToken = new CancellationToken()) {
            return SyntaxTree.FromFile(filePath, cancellationToken);
        }

        public virtual void Dispose() {
        }
    }
}