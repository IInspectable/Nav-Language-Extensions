﻿#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;
using Pharmatechnik.Nav.Language.Extension.Images;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class IntraTextGoToTagSpanBuilder: NavTaskAnnotationVisitor<ITagSpan<IntraTextGoToTag>> {

        const string ToolTipGoToTaskDefinition    = "Go To Task Definition";
        const string ToolTipGoToInitDefinition    = "Go To Init Definition";
        const string ToolTipGoToExitDefinition    = "Go To Exit Definition";
        const string ToolTipGoToTriggerDefinition = "Go To Trigger Definition";
        const string ToolTipGoToImplementation    = "Go To Implementation";

        readonly ImmutableList<NavTaskAnnotation> _allAnnotations;
        readonly ITextSnapshot                    _textSnapshot;

        public IntraTextGoToTagSpanBuilder(IEnumerable<NavTaskAnnotation> allAnnotations,
                                           ITextSnapshot textSnapshot) {
            _allAnnotations = allAnnotations.ToImmutableList();
            _textSnapshot   = textSnapshot;
        }

        public override ITagSpan<IntraTextGoToTag> VisitNavTaskAnnotation(NavTaskAnnotation navTaskAnnotation) {

            var start  = navTaskAnnotation.ClassDeclarationSyntax.Identifier.Span.Start;
            var length = navTaskAnnotation.ClassDeclarationSyntax.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);
            var provider     = new NavTaskAnnotationLocationInfoProvider(navTaskAnnotation);
            var tag = new IntraTextGoToTag(
                provider    : provider, 
                imageMoniker: ImageMonikers.GoToDefinition,
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
                imageMoniker: ImageMonikers.GoToDefinition, 
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
                imageMoniker: ImageMonikers.GoToDefinition, 
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
                imageMoniker: ImageMonikers.GoToDefinition, 
                toolTip     : ToolTipGoToTriggerDefinition);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }

        public override ITagSpan<IntraTextGoToTag> VisitNavInitCallAnnotation(NavInitCallAnnotation navInitCallAnnotation) {

            var start  = navInitCallAnnotation.Identifier.Span.Start;
            var length = navInitCallAnnotation.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(_textSnapshot, start, length);

            var navExitAnnotation = _allAnnotations.OfType<NavExitAnnotation>()
                                                   .First(a => a.TaskName == navInitCallAnnotation.TaskName);

            var provider = new NavInitCallLocationInfoProvider(
                sourceBuffer  : _textSnapshot.TextBuffer, 
                callAnnotation: navInitCallAnnotation, 
                exitAnnotation: navExitAnnotation);

            var tag = new IntraTextGoToTag(
                provider      : provider,
                imageMoniker  : ImageMonikers.GoToDeclaration,
                toolTip       : ToolTipGoToImplementation);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, tag);
        }
    }
}