#region Using Directives

using System.IO;
using System.Threading;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class SyntaxProvider: ISyntaxProvider {

        public static readonly ISyntaxProvider Default = new SyntaxProvider();

        public virtual CodeGenerationUnitSyntax FromFile(string filePath, CancellationToken cancellationToken = default) {

            if (!File.Exists(filePath)) {
                return null;
            }

            var content    = File.ReadAllText(filePath);
            var syntaxTree = Syntax.ParseCodeGenerationUnit(text: content, filePath: filePath, cancellationToken: cancellationToken);

            return syntaxTree;
        }

        public virtual void Dispose() {
        }

    }

}