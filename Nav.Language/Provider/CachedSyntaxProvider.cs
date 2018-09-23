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

    public class CachedSyntaxProvider: ISyntaxProvider {

        readonly Dictionary<string, CodeGenerationUnitSyntax> _cache;
        readonly Dictionary<string, int>                      _cacheStatistic;

        private readonly ISyntaxProvider _syntaxProvider;

        public CachedSyntaxProvider(): this(null) {

        }

        public CachedSyntaxProvider(ISyntaxProvider syntaxProvider) {

            _syntaxProvider = syntaxProvider ?? SyntaxProvider.Default;
            _cache          = new Dictionary<string, CodeGenerationUnitSyntax>(StringComparer.OrdinalIgnoreCase);
            _cacheStatistic = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public virtual CodeGenerationUnitSyntax GetSyntax(string filePath, CancellationToken cancellationToken = default) {

            if (_cache.TryGetValue(filePath, out var syntax)) {
                if (_cacheStatistic.ContainsKey(filePath)) {
                    _cacheStatistic[filePath] += 1;
                } else {
                    _cacheStatistic[filePath] = 1;
                }

                return syntax;
            }

            syntax = _syntaxProvider.GetSyntax(filePath, cancellationToken);
            Cache(filePath, syntax);

            return syntax;
        }

        public CachedSyntaxProviderStatistic GetStatistic() {
            return new CachedSyntaxProviderStatistic(_cacheStatistic.Sum(kvp => kvp.Value));
        }

        public virtual void Dispose() {
            ClearCache();
        }

        void Cache(string filePath, CodeGenerationUnitSyntax syntax) {
            _cache[filePath] = syntax;
        }

        void ClearCache() {
            _cache.Clear();
            _cacheStatistic.Clear();
        }

    }

}