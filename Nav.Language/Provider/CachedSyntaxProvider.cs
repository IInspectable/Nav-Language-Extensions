#region Using Directives

using System;
using System.Collections.Concurrent;
using System.Threading;

#endregion

namespace Pharmatechnik.Nav.Language {

    public struct CachedSyntaxProviderStatistic {

        public CachedSyntaxProviderStatistic(int cacheHits, int cacheFails) {
            CacheHits  = cacheHits;
            CacheFails = cacheFails;
        }

        public int CacheHits  { get; }
        public int CacheFails { get; }

        public CachedSyntaxProviderStatistic WithCacheHit() {
            return new CachedSyntaxProviderStatistic(CacheHits + 1, CacheFails);
        }

        public CachedSyntaxProviderStatistic WithCacheFail() {
            return new CachedSyntaxProviderStatistic(CacheHits, CacheFails + 1);
        }

    }

    public class CachedSyntaxProvider: ISyntaxProvider {

        readonly ConcurrentDictionary<string, CodeGenerationUnitSyntax> _cache;
        readonly ISyntaxProvider                                        _syntaxProvider;

        private readonly object _gate = new object();

        public CachedSyntaxProvider(): this(null) {

        }

        public CachedSyntaxProvider(ISyntaxProvider syntaxProvider) {

            _syntaxProvider = syntaxProvider ?? SyntaxProvider.Default;
            _cache          = new ConcurrentDictionary<string, CodeGenerationUnitSyntax>(StringComparer.OrdinalIgnoreCase);
            Statistic       = default;
        }

        public virtual CodeGenerationUnitSyntax GetSyntax(string filePath, CancellationToken cancellationToken = default) {

            if (_cache.TryGetValue(filePath, out var syntax)) {

                CacheHit();
                return syntax;
            }

            CacheFail();

            syntax = _syntaxProvider.GetSyntax(filePath, cancellationToken);

            Cache(filePath, syntax);

            return syntax;
        }

        public CachedSyntaxProviderStatistic Statistic { get; private set; }

        public virtual void Dispose() {
            ClearCache();
        }

        void CacheFail() {

            lock (_gate) {
                Statistic = Statistic.WithCacheFail();
            }
        }

        void CacheHit() {

            lock (_gate) {
                Statistic = Statistic.WithCacheHit();
            }
        }

        void Cache(string filePath, CodeGenerationUnitSyntax syntax) {
            _cache[filePath] = syntax;
        }

        void ClearCache() {
            lock (_gate) {
                _cache.Clear();
                Statistic = default;
            }
        }

    }

}