#region Using Directives

using System.Collections.Generic;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    partial class CodeFixSuggestedActionsSource {

        sealed class SuggestedActionSetComparer : IComparer<SuggestedActionSet> {

            readonly SnapshotPoint? _triggerPoint;

            public SuggestedActionSetComparer(SnapshotPoint? triggerPoint) {
                _triggerPoint = triggerPoint;
            }

            private static int Distance(Span? textSpan, SnapshotPoint? targetPoint) {
                // If we don't have a text span or target point we cannot calculate the distance between them
                if(textSpan == null || targetPoint == null) {
                    return int.MaxValue;
                }

                var span = textSpan.Value;
                var position = targetPoint.Value.Position;

                if(position < span.Start) {
                    return span.Start - position;
                } else if(position > span.End) {
                    return position - span.End;
                } else {
                    return 0;
                }
            }

            public int Compare(SuggestedActionSet x, SuggestedActionSet y) {
                if(_triggerPoint?.Position == null || x?.ApplicableToSpan == null || y?.ApplicableToSpan == null) {
                    // Not enough data to compare, consider them equal
                    return 0;
                }

                var distanceX = Distance(x.ApplicableToSpan, _triggerPoint);
                var distanceY = Distance(y.ApplicableToSpan, _triggerPoint);

                if(distanceX != 0 || distanceY != 0) {
                    return distanceX.CompareTo(distanceY);
                }

                // This is the case when both actions sets' spans contain the trigger point.
                // Now we compare first by start position then by end position. 
                // ReSharper disable once PossibleInvalidOperationException Kann nicht null sein Wegen Distance Aufruf
                var targetPosition = _triggerPoint.Value.Position;

                var distanceToStartX = targetPosition - x.ApplicableToSpan.Value.Start;
                var distanceToStartY = targetPosition - y.ApplicableToSpan.Value.Start;

                if(distanceToStartX != distanceToStartY) {
                    return distanceToStartX.CompareTo(distanceToStartY);
                }

                var distanceToEndX = x.ApplicableToSpan.Value.End - targetPosition;
                var distanceToEndY = y.ApplicableToSpan.Value.End - targetPosition;

                return distanceToEndX.CompareTo(distanceToEndY);
            }
        }
    }
}