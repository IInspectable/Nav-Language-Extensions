#region Using Directives

using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class CachedSyntaxProvider : SyntaxProvider {

        readonly Dictionary<string, SyntaxTree> _cache;

        public CachedSyntaxProvider() {
            _cache = new Dictionary<string, SyntaxTree>();
        }

        public override SyntaxTree FromFile(string filePath, CancellationToken cancellationToken = default(CancellationToken)) {

            if(!_cache.TryGetValue(filePath, out var syntaxTree)) {
                syntaxTree = base.FromFile(filePath, cancellationToken);
                Cache(filePath, syntaxTree);
            }

            return syntaxTree;
        }

        public void Cache(string filePath, SyntaxTree syntaxTree) {
            _cache[filePath] = syntaxTree;
        }

        public void ClearCache() {
            _cache.Clear();
        }

        public override void Dispose() {
            base.Dispose();
            ClearCache();
        }
    }

}