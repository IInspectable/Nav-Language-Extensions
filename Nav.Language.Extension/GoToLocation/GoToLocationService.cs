#region Using Directives

using System;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls.Primitives;

using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Nav.Language.Extension.CodeAnalysis;
using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Language.Extension.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {
    
    [Export]
    sealed class GoToLocationService {

        // TODO Titel etc. überarbeiten
        const string WaitIndicatorTitle = "Nav Language Extensions";
        const string SearchingLocationMessage = "Searching Location...";
        const string OpeningFileMessage = "Opening file...";

        readonly IWaitIndicator _waitIndicator;

        [ImportingConstructor]
        public GoToLocationService(IWaitIndicator waitIndicator) {
            _waitIndicator = waitIndicator;
        }

        public async Task GoToLocationInPreviewTabAsync(IWpfTextView originatingTextView, Func<CancellationToken, Task<IEnumerable<LocationResult>>> getLocationsTask) {
            
            List<LocationResult> locations;
            using (var waitContext = _waitIndicator.StartWait(title: WaitIndicatorTitle, message: SearchingLocationMessage, allowCancel: true)) {

                try {

                    var task = getLocationsTask(waitContext.CancellationToken);
                    locations = (await task).ToList();

                    // TODO Ist dieser Check nötig?
                    if (task.IsCanceled) {
                        return;
                    }

                    locations.Add(locations[0]); // Zum Simulieren mehrerer Locations

                    // Es gibt nur eine einzige Location => direkt anspringen, da wir denselben Wait Indicator verwenden wollen.
                    if (locations.Count == 1 && locations[0].Location != null) {

                        var locationResult = locations.First();

                        waitContext.AllowCancel = false;
                        waitContext.Message = OpeningFileMessage;

                       NavLanguagePackage.GoToLocationInPreviewTab(locationResult.Location);
                        return;
                    }
                } catch (TaskCanceledException) {
                    return;
                }
            }

            if (locations.Count == 0) {
                return;
            }

            // Es gibt nur eine Location, die aber nicht aufgelöst werden konnte => Fehler anzeigen und tschüss
            if (locations.Count == 1 && !String.IsNullOrWhiteSpace(locations[0].ErrorMessage)) {
                MessageBox.Show(locations[0].ErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            // Wenn wir hier sind, dann gibt es mehrere Locations, für die wir eine Auswahl anzeigen müssen
            ContextMenu ctxMenu = new ContextMenu {
                PlacementTarget = originatingTextView.VisualElement,
                // _view.TextViewLines.GetMarkerGeometry(span = new SnapshotSpan(_view.TextSnapshot,
                PlacementRectangle = new Rect(new Point(100, 100), new Size(22, 22)), // TODO muss von außen kommen
                Placement = PlacementMode.Bottom,
                StaysOpen = false,
                IsOpen = true
            };

            foreach (var location in locations) {
                MenuItem item = new MenuItem { Header = location.Location.FilePath };
                item.Click+=(o,e)=> GoToLocationInPreviewTab(location);
                ctxMenu.Items.Add(item);
            }
        }
        
        void GoToLocationInPreviewTab(LocationResult location) {
            using (_waitIndicator.StartWait(title: WaitIndicatorTitle, message: OpeningFileMessage, allowCancel: true)) {
                NavLanguagePackage.GoToLocationInPreviewTab(location.Location);
            }
        }
    }
}