#region Using Directives

using System.Collections.Generic;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    partial class CodeFixSuggestedActionsSource {

        sealed class SuggestedActionSetComparer : IComparer<SuggestedActionSet> {

            readonly SnapshotPoint? _triggerPoint;
            readonly SnapshotSpan   _defaultSpan;

            public SuggestedActionSetComparer(SnapshotPoint? triggerPoint, SnapshotSpan defaultSpan) {
                
                _triggerPoint = triggerPoint;
                _defaultSpan = defaultSpan;
            }

            int Distance(Span span) {
                // If we don't have a text span or target point we cannot calculate the distance between them
                if(_triggerPoint == null) {
                    return int.MaxValue;
                }

                var position = _triggerPoint.Value.Position;

                if(position < span.Start) {
                    return span.Start - position;
                } else if(position > span.End) {
                    return position - span.End;
                } else {
                    return 0;
                }
            }

            public int Compare(SuggestedActionSet x, SuggestedActionSet y) {
                var triggerPoint = _triggerPoint;
                if (triggerPoint == null) {
                    // Ohne Triggerpoint kann keine Aussage getroffen werden
                    return 0;
                }

                var xSpan = x?.ApplicableToSpan ?? _defaultSpan.Span;
                var ySpan = y?.ApplicableToSpan ?? _defaultSpan.Span;

                var distanceX = Distance(xSpan);
                var distanceY = Distance(ySpan);

                if(distanceX != 0 || distanceY != 0) {
                    return distanceX.CompareTo(distanceY);
                }

                // This is the case when both actions sets' spans contain the trigger point.
                // Now we compare first by start position then by end position. 
                var triggerPosition = triggerPoint.Value.Position;

                var distanceToStartX = triggerPosition - xSpan.Start;
                var distanceToStartY = triggerPosition - ySpan.Start;

                if(distanceToStartX != distanceToStartY) {
                    return distanceToStartX.CompareTo(distanceToStartY);
                }

                var distanceToEndX = xSpan.End - triggerPosition;
                var distanceToEndY = ySpan.End - triggerPosition;

                return distanceToEndX.CompareTo(distanceToEndY);
            }
        }
    }
}