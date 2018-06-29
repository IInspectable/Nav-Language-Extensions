#region Using Directives

using System;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls.Primitives;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Utilities.Logging;
using Pharmatechnik.Nav.Language.Extension.UI;
using Pharmatechnik.Nav.Language.Extension.Utilities;
using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

using Task = System.Threading.Tasks.Task;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {
    
    [Export]
    sealed class GoToLocationService {

        static readonly Logger Logger = Logger.Create<GoToLocationService>();

        const string MessageTitle             = "Nav Language Extensions";
        const string SearchingLocationMessage = "Searching Location...";
        const string OpeningFileMessage       = "Opening file...";
        const string ContextMenuHeader        = "Go To...";

        readonly IWaitIndicator _waitIndicator;

        [ImportingConstructor]
        public GoToLocationService(IWaitIndicator waitIndicator) {
            _waitIndicator = waitIndicator;
        }

        public async Task GoToLocationInPreviewTabAsync(IWpfTextView originatingTextView, Rect placementRectangle, IEnumerable<ILocationInfoProvider> provider) {
            
            List<LocationInfo> locationInfos;
            using (var waitContext = _waitIndicator.StartWait(title: MessageTitle, message: SearchingLocationMessage, allowCancel: true)) {

                try {

                    var locs = await GetLocationInfosAsync(provider, waitContext.CancellationToken);
                    locationInfos = locs.ToList();

                    // Es gibt nur eine einzige Location => direkt anspringen, da wir denselben Wait Indicator verwenden wollen.
                    if (locationInfos.Count == 1 && locationInfos[0].IsValid) {

                        var locationResult = locationInfos.First();

                        waitContext.AllowCancel = false;
                        waitContext.Message     = OpeningFileMessage;

                        NavLanguagePackage.GoToLocationInPreviewTab(locationResult.Location);

                        return;
                    }
                } catch (OperationCanceledException) {
                    return;
                }
            }

            if (locationInfos.Count == 0) {
                return;
            }

            // Es gibt nur eine Location, die aber nicht aufgelöst werden konnte => Fehler anzeigen und tschüss
            if (locationInfos.Count == 1 && !locationInfos[0].IsValid) {
                ShowLocationErrorMessage(locationInfos[0]);
                return;
            }

            // Wenn wir hier sind, dann gibt es mehrere Locations, für die wir eine Auswahl anzeigen müssen
            var ctxMenu = new VsContextMenu {
                Header             = ContextMenuHeader,
                PlacementTarget    = originatingTextView.VisualElement,
                PlacementRectangle = placementRectangle, 
                Placement          = PlacementMode.Bottom,
                StaysOpen          = false,
                IsOpen             = true
            };

            foreach (var locationInfo in locationInfos) {
               
                var item = new VsMenuItem {
                    Header    = locationInfo.IsValid? locationInfo.DisplayName:locationInfo.ErrorMessage,
                    IsEnabled = locationInfo.IsValid,
                    Icon      = new CrispImage {
                        Moniker   = locationInfo.ImageMoniker,
                        Grayscale = !locationInfo.IsValid
                    },
                    //InputGestureText = "<XTPlus.OffenePosten>"
                };
                item.Click += (_, __) => GoToLocationInPreviewTab(locationInfo);

                ctxMenu.Items.Add(item);
            }
        }

        static async Task<IEnumerable<LocationInfo>> GetLocationInfosAsync(IEnumerable<ILocationInfoProvider> providers, CancellationToken cancellationToken = default) {
            using(Logger.LogBlock(nameof(GetLocationInfosAsync))) {

                var results = await Task.WhenAll(providers.Select(p => p.GetLocationsAsync(cancellationToken)));

                return results.SelectMany(x => x);
            }
        }

        void GoToLocationInPreviewTab(LocationInfo locationInfo) {
            
            ThreadHelper.ThrowIfNotOnUIThread();

            if(!locationInfo.IsValid) {
                ShowLocationErrorMessage(locationInfo);
                return;
            }

            using(_waitIndicator.StartWait(title: MessageTitle, message: OpeningFileMessage, allowCancel: false)) {
                NavLanguagePackage.GoToLocationInPreviewTab(locationInfo.Location);
            }
        }

        void ShowLocationErrorMessage(LocationInfo locationInfo) {
            ShellUtil.ShowErrorMessage(locationInfo.ErrorMessage);
        }
    }
}