#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;

using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class IntraTextGoToTagSpanBuilder: NavTaskAnnotationVisitor<TagSpan<IntraTextGoToTag>> {

        readonly ITextSnapshot _textSnapshot;
        
        public IntraTextGoToTagSpanBuilder(ITextSnapshot textSnapshot) {
            _textSnapshot = textSnapshot;
        }

        public override TagSpan<IntraTextGoToTag> VisitNavTaskAnnotation(NavTaskAnnotation navTaskAnnotation) {

            var start  = navTaskAnnotation.ClassDeclarationSyntax.Identifier.Span.Start;
            var length = navTaskAnnotation.ClassDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavTaskAnnotationLocationInfoProvider(navTaskAnnotation);
            // TODO Tooltip in Ressource
            var tag = new IntraTextGoToTag(provider, navTaskAnnotation) {
                ToolTip      = "Go To Task Definition",
                ImageMoniker = GoToImageMonikers.Definition
            };
            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }

        public override TagSpan<IntraTextGoToTag> VisitNavInitAnnotation(NavInitAnnotation navInitAnnotation) {

            int start  = navInitAnnotation.MethodDeclarationSyntax.Identifier.Span.Start;
            int length = navInitAnnotation.MethodDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavInitAnnotationLocationInfoProvider(navInitAnnotation);
            // TODO Tooltip in Ressource
            var tag = new IntraTextGoToTag(provider, navInitAnnotation) {
                ToolTip      = "Go To Init Definition",
                ImageMoniker = GoToImageMonikers.Definition
            };
            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }

        public override TagSpan<IntraTextGoToTag> VisitNavExitAnnotation(NavExitAnnotation navExitAnnotation) {

            int start  = navExitAnnotation.MethodDeclarationSyntax.Identifier.Span.Start;
            int length = navExitAnnotation.MethodDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavExitAnnotationLocationInfoProvider(navExitAnnotation);
            // TODO Tooltip in Ressource
            var tag = new IntraTextGoToTag(provider, navExitAnnotation) {
                ToolTip      = "Go To Exit Definition",
                ImageMoniker = GoToImageMonikers.Definition
            };
            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }

        public override TagSpan<IntraTextGoToTag> VisitNavTriggerAnnotation(NavTriggerAnnotation navTriggerAnnotation) {

            int start  = navTriggerAnnotation.MethodDeclarationSyntax.Identifier.Span.Start;
            int length = navTriggerAnnotation.MethodDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavTriggerAnnotationLocationInfoProvider(navTriggerAnnotation);
            // TODO Tooltip in Ressource
            var tag = new IntraTextGoToTag(provider, navTriggerAnnotation) {
                ToolTip      = "Go To Trigger Definition",
                ImageMoniker = GoToImageMonikers.Definition
            };
            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }
        
        public override TagSpan<IntraTextGoToTag> VisitNavInitCallAnnotation(NavInitCallAnnotation navInitCallAnnotation) {

            var start  = navInitCallAnnotation.Identifier.Span.Start;
            var length = navInitCallAnnotation.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);

            var provider = new NavInitCallLocationInfoProvider(_textSnapshot.TextBuffer, navInitCallAnnotation);
            // TODO Tooltip in Ressource
            var tag = new IntraTextGoToTag(provider, navInitCallAnnotation) {
                ToolTip      = "Go To Implementation",
                ImageMoniker = GoToImageMonikers.Declaration
            };
            
            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }
    }
}
