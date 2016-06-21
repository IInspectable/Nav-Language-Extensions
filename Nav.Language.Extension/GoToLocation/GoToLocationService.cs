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
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

using Pharmatechnik.Nav.Language.Extension.UI;
using Pharmatechnik.Nav.Language.Extension.Utilities;
using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {
    
    [Export]
    sealed class GoToLocationService {

        // TODO Titel etc. überarbeiten
        const string MessageTitle             = "Nav Language Extensions";
        const string SearchingLocationMessage = "Searching Location...";
        const string OpeningFileMessage       = "Opening file...";

        readonly IWaitIndicator _waitIndicator;

        [ImportingConstructor]
        public GoToLocationService(IWaitIndicator waitIndicator) {
            _waitIndicator = waitIndicator;
        }

        public async Task GoToLocationInPreviewTabAsync(IWpfTextView originatingTextView, Rect placementRectangle, IEnumerable<ILocationInfoProvider> provider) {
            
            List<LocationInfo> locations;
            using (var waitContext = _waitIndicator.StartWait(title: MessageTitle, message: SearchingLocationMessage, allowCancel: true)) {

                try {

                    var locs = await GetLocationsAsync(provider, waitContext.CancellationToken);
                    locations = locs.ToList();

                    // Es gibt nur eine einzige Location => direkt anspringen, da wir denselben Wait Indicator verwenden wollen.
                    if (locations.Count == 1 && locations[0].IsValid) {

                        var locationResult = locations.First();

                        waitContext.AllowCancel = false;
                        waitContext.Message     = OpeningFileMessage;

                        NavLanguagePackage.GoToLocationInPreviewTab(locationResult.Location);

                        return;
                    }
                } catch (OperationCanceledException) {
                    return;
                }
            }

            if (locations.Count == 0) {
                return;
            }

            // Es gibt nur eine Location, die aber nicht aufgelöst werden konnte => Fehler anzeigen und tschüss
            if (locations.Count == 1 && !locations[0].IsValid) {
                ShowLocationErrorMessage(locations[0]);
                return;
            }

            // Wenn wir hier sind, dann gibt es mehrere Locations, für die wir eine Auswahl anzeigen müssen
            var ctxMenu = new VsContextMenu {
                Header             = "Go To...",
                PlacementTarget    = originatingTextView.VisualElement,
                PlacementRectangle = placementRectangle, 
                Placement          = PlacementMode.Bottom,
                StaysOpen          = false,
                IsOpen             = true
            };

            foreach (var location in locations) {

                var crispImage = new CrispImage {
                    Moniker = GoToImageMonikers.GetMoniker(location.Kind)
                };

                var item = new VsMenuItem {
                    Header = location.DisplayName,
                    Icon   = crispImage ,
                    //InputGestureText = "<XTPlus.OffenePosten>"
                };
                item.Click += (_, __) => GoToLocationInPreviewTab(location);

                ctxMenu.Items.Add(item);
            }
        }

        static async Task<IEnumerable<LocationInfo>> GetLocationsAsync(IEnumerable<ILocationInfoProvider> providers, CancellationToken cancellationToken = default(CancellationToken)) {

            var locationInfos = new List<LocationInfo>();

            foreach (var provider in providers) {
                var lis = await provider.GetLocationsAsync(cancellationToken);

                locationInfos.AddRange(lis);
            }

            return locationInfos;
        }

        void GoToLocationInPreviewTab(LocationInfo location) {

            if(!location.IsValid) {
                ShowLocationErrorMessage(location);
                return;
            }

            using(_waitIndicator.StartWait(title: MessageTitle, message: OpeningFileMessage, allowCancel: false)) {
                NavLanguagePackage.GoToLocationInPreviewTab(location.Location);
            }
        }

        void ShowLocationErrorMessage(LocationInfo locationInfo) {
            MessageBox.Show(messageBoxText: locationInfo.ErrorMessage, caption: MessageTitle, button: MessageBoxButton.OK, icon: MessageBoxImage.Asterisk);
        }
    }
}