#region Using Directives

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Classification;

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Formatting;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    sealed class GoToDefinitionAdorner {

        #region Fields/Ctor

        public const string AdornerName = "Nav/NavigateToAdorner";

        readonly IWpfTextView    _textView;
        readonly IAdornmentLayer _adornmentLayer;
        readonly IClassifier     _classifier;
        readonly IClassificationFormatMapService _classicationFormatMapService;

        SnapshotSpan _currentSpan;

        GoToDefinitionAdorner(IWpfTextView textTextView, IComponentModel componentModel) {
            _textView = textTextView;

            _classicationFormatMapService = componentModel.GetService<IClassificationFormatMapService>();
            _classifier                   = componentModel.GetService<IViewClassifierAggregatorService>().GetClassifier(textTextView);            
            _adornmentLayer               = textTextView.GetAdornmentLayer(AdornerName);

            _textView.Closed += OnTextViewClosed;
        }

        #endregion

        void OnTextViewClosed(object sender, EventArgs e) {
            _textView.Properties.RemoveProperty(GetType());
            _textView.Closed -= OnTextViewClosed;
        }

        public static GoToDefinitionAdorner GetOrCreate(IWpfTextView textView) {
            var componentModel = NavLanguagePackage.GetGlobalService<SComponentModel, IComponentModel>();
            return textView.Properties.GetOrCreateSingletonProperty(() =>
                new GoToDefinitionAdorner(textView, componentModel));
        }
        
        public void RemoveNavigateToSpan() {

            _adornmentLayer.RemoveAllAdornments();

            if(!_currentSpan.IsEmpty) {
                _currentSpan = new SnapshotSpan();
                //Mouse.OverrideCursor = null;
            }
        }

        public void SetNavigateToSpan(SnapshotSpan span, Action mouseLeftButtonUpAction) {

            if(span == _currentSpan) {
                //Mouse.OverrideCursor = span.IsEmpty ? null : Cursors.Hand;
                return;
            }

            RemoveNavigateToSpan();

            if (span.IsEmpty) {                
                return;
            }

            _currentSpan = span;
            //Mouse.OverrideCursor = Cursors.Hand;

            var geom   = _textView.TextViewLines.GetTextMarkerGeometry(span);
            var bounds = geom.Bounds;

            var underlineProps     = GetFormattingProperties(span);
            var underlineThickness = 1;

            var underlineBrush     = underlineProps?.ForegroundBrush ?? Brushes.Red;
            var underlineRect      = new Rect(bounds.Left, 
                                              bounds.Bottom - underlineThickness, 
                                              bounds.Width, 
                                              underlineThickness);

            var underline = new Path {
                Fill                = underlineBrush,
                Data                = new RectangleGeometry(underlineRect),
                SnapsToDevicePixels = true,
                Cursor              = Cursors.Hand,
            };
            underline.MouseLeftButtonUp += (o, e) => mouseLeftButtonUpAction();

            var rectangle = new Path {
                Fill                = Brushes.Transparent,
                Data                = new RectangleGeometry(bounds),
                SnapsToDevicePixels = true,
                Cursor              = Cursors.Hand,
            };
            rectangle.MouseLeftButtonUp += (o, e) => mouseLeftButtonUpAction();

            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, this, rectangle, null);
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, this, underline, null);
        }

        [CanBeNull]
        TextFormattingRunProperties GetFormattingProperties(SnapshotSpan span) {

            var classificationMap = _classicationFormatMapService.GetClassificationFormatMap(_textView);
            var classifications   = _classifier.GetClassificationSpans(span);
            var classification    = classifications.FirstOrDefault();

            if(classification == null) {
                // Sollte eigentlich nicht vorkommen...
                return null;
            }

            var textProperties = classificationMap.GetTextProperties(classification.ClassificationType);
            
            return textProperties;
        }
    }
}
