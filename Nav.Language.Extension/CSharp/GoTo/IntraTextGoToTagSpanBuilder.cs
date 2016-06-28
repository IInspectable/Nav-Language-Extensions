#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;

using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class IntraTextGoToTagSpanBuilder: NavTaskAnnotationVisitor<ITagSpan<IntraTextGoToTag>> {

        const string ToolTipGoToTaskDefinition    = "Go To Task Definition";
        const string ToolTipGoToInitDefinition    = "Go To Init Definition";
        const string ToolTipGoToExitDefinition    = "Go To Exit Definition";
        const string ToolTipGoToTriggerDefinition = "Go To Trigger Definition";
        const string ToolTipGoToImplementation    = "Go To Implementation";

        readonly ITextSnapshot _textSnapshot;
        
        public IntraTextGoToTagSpanBuilder(ITextSnapshot textSnapshot) {
            _textSnapshot = textSnapshot;
        }

        public override ITagSpan<IntraTextGoToTag> VisitNavTaskAnnotation(NavTaskAnnotation navTaskAnnotation) {

            var start  = navTaskAnnotation.ClassDeclarationSyntax.Identifier.Span.Start;
            var length = navTaskAnnotation.ClassDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavTaskAnnotationLocationInfoProvider(navTaskAnnotation);
            var tag = new IntraTextGoToTag(
                provider    : provider, 
                imageMoniker: GoToImageMonikers.Definition,
                toolTip     : ToolTipGoToTaskDefinition);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }

        public override ITagSpan<IntraTextGoToTag> VisitNavInitAnnotation(NavInitAnnotation navInitAnnotation) {

            int start  = navInitAnnotation.MethodDeclarationSyntax.Identifier.Span.Start;
            int length = navInitAnnotation.MethodDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavInitAnnotationLocationInfoProvider(navInitAnnotation);
            var tag = new IntraTextGoToTag(
                provider    : provider, 
                imageMoniker: GoToImageMonikers.Definition, 
                toolTip     : ToolTipGoToInitDefinition);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }

        public override ITagSpan<IntraTextGoToTag> VisitNavExitAnnotation(NavExitAnnotation navExitAnnotation) {

            int start  = navExitAnnotation.MethodDeclarationSyntax.Identifier.Span.Start;
            int length = navExitAnnotation.MethodDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavExitAnnotationLocationInfoProvider(navExitAnnotation);
            var tag = new IntraTextGoToTag(
                provider    : provider, 
                imageMoniker: GoToImageMonikers.Definition, 
                toolTip     : ToolTipGoToExitDefinition);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }

        public override ITagSpan<IntraTextGoToTag> VisitNavTriggerAnnotation(NavTriggerAnnotation navTriggerAnnotation) {

            int start  = navTriggerAnnotation.MethodDeclarationSyntax.Identifier.Span.Start;
            int length = navTriggerAnnotation.MethodDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavTriggerAnnotationLocationInfoProvider(navTriggerAnnotation);
            var tag = new IntraTextGoToTag(
                provider    : provider, 
                imageMoniker: GoToImageMonikers.Definition, 
                toolTip     : ToolTipGoToTriggerDefinition);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }
        
        public override ITagSpan<IntraTextGoToTag> VisitNavInitCallAnnotation(NavInitCallAnnotation navInitCallAnnotation) {

            var start  = navInitCallAnnotation.Identifier.Span.Start;
            var length = navInitCallAnnotation.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);

            var provider = new NavInitCallLocationInfoProvider(_textSnapshot.TextBuffer, navInitCallAnnotation);
            var tag = new IntraTextGoToTag(
                provider    : provider, 
                imageMoniker: GoToImageMonikers.Declaration, 
                toolTip     : ToolTipGoToImplementation);      
                  
            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }
    }
}