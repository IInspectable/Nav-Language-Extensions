#region Using Directives

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion3 {

    class CompletionSource: IAsyncCompletionSource {

        public bool TryGetApplicableToSpan(char typedChar, SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, CancellationToken token) {
            applicableToSpan = default;
            return false;
        }

        public Task<CompletionContext> GetCompletionContextAsync(InitialTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token) {

            //var imageId      =  KnownMonikers.TaskList.ToImageId();
            //var imageElement = new ImageElement(imageId);

            var itemsBuilder = ImmutableArray.CreateBuilder<CompletionItem>();

            var context = new CompletionContext(itemsBuilder.ToImmutable());
            return Task.FromResult(context);
        }

        public Task<object> GetDescriptionAsync(CompletionItem item, CancellationToken token) {
            return Task.FromResult((object) item.DisplayText);
        }

    }

}