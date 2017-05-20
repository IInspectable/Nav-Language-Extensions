#region Using Directives

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class CachedSyntaxProviderStatistic {

        public CachedSyntaxProviderStatistic(int cacheHits) {
            CacheHits = cacheHits;
        }
        public int CacheHits { get; }        
    }

    public class CachedSyntaxProvider : SyntaxProvider {

        readonly Dictionary<string, SyntaxTree> _cache;
        readonly Dictionary<string, int> _cacheStatistic;

        public CachedSyntaxProvider() {
            _cache = new Dictionary<string, SyntaxTree>(StringComparer.OrdinalIgnoreCase);
            _cacheStatistic = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public override SyntaxTree FromFile(string filePath, CancellationToken cancellationToken = default(CancellationToken)) {

            if(_cache.TryGetValue(filePath, out var syntaxTree)) {
                _cacheStatistic[filePath] += 1;
                return syntaxTree;
            }

            syntaxTree = base.FromFile(filePath, cancellationToken);
            Cache(filePath, syntaxTree);

            return syntaxTree;
        }

        public CachedSyntaxProviderStatistic GetStatistic() {
            return new CachedSyntaxProviderStatistic(_cacheStatistic.Sum(kvp => kvp.Value));
        }

        public override void Dispose() {
            base.Dispose();
            ClearCache();
        }

        void Cache(string filePath, SyntaxTree syntaxTree) {
            _cache[filePath] = syntaxTree;
        }

        void ClearCache() {
            _cache.Clear();
            _cacheStatistic.Clear();
        }                    
    }
}