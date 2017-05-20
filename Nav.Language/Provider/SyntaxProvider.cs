#region Using Directives

using System.IO;
using System.Threading;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class SyntaxProvider : ISyntaxProvider {
        
        public static readonly ISyntaxProvider Default = new SyntaxProvider();

        public virtual SyntaxTree FromFile(string filePath, CancellationToken cancellationToken = new CancellationToken()) {

            if (!File.Exists(filePath)) {            
                return null;
            }

            // TODO Try catch?
            var syntaxTree = SyntaxTree.FromFile(filePath, cancellationToken);

            return syntaxTree;
        }

        public virtual void Dispose() {
        }
    }
}