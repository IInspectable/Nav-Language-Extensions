#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public abstract class Generator: IDisposable {

        protected Generator(GenerationOptions options) {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public GenerationOptions Options { get; }

        public virtual void Dispose() {
        }
    }
}