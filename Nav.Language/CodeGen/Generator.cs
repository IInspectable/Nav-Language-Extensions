#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public abstract class Generator: IDisposable {

        protected Generator(GenerationOptions options) {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }
        [NotNull]
        public GenerationOptions Options { get; }

        public virtual void Dispose() {
        }
    }
}