#region Using Directives

using System;
using System.Threading;
using System.Collections.Generic;

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

        readonly Dictionary<string, CodeGenerationUnitSyntax> _cache;
        readonly ISyntaxProvider                              _syntaxProvider;

        public CachedSyntaxProvider(): this(null) {

        }

        public CachedSyntaxProvider(ISyntaxProvider syntaxProvider) {

            _syntaxProvider = syntaxProvider ?? SyntaxProvider.Default;
            _cache          = new Dictionary<string, CodeGenerationUnitSyntax>(StringComparer.OrdinalIgnoreCase);
            Statistic       = default;
        }

        public virtual CodeGenerationUnitSyntax GetSyntax(string filePath, CancellationToken cancellationToken = default) {

            if (_cache.TryGetValue(filePath, out var syntax)) {

                Statistic = Statistic.WithCacheHit();

                return syntax;
            }

            Statistic = Statistic.WithCacheFail();

            syntax = _syntaxProvider.GetSyntax(filePath, cancellationToken);
            Cache(filePath, syntax);

            return syntax;
        }

        public CachedSyntaxProviderStatistic Statistic { get; private set; }

        public virtual void Dispose() {
            ClearCache();
        }

        void Cache(string filePath, CodeGenerationUnitSyntax syntax) {
            _cache[filePath] = syntax;
        }

        void ClearCache() {
            _cache.Clear();
            Statistic = default;
        }

    }

}