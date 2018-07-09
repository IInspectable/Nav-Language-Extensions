#region

using System.Collections.Immutable;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    sealed class NavigationBarItem {

        public NavigationBarItem(string displayName, int imageIndex): this(displayName, imageIndex, null, -1) {
        }

        public NavigationBarItem(string displayName, int imageIndex, [CanBeNull] Location location, int navigationPoint, ImmutableList<NavigationBarItem> children = null) {
            Location        = location;
            NavigationPoint = navigationPoint;
            DisplayName     = displayName;
            ImageIndex      = imageIndex;
            Children        = children ?? ImmutableList<NavigationBarItem>.Empty;
        }

        /// <summary>
        /// Liefert den Anzeigenamen
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Liefert den Image Index für das item.
        /// </summary>
        public int ImageIndex { get; }

        public int StartLine => Location?.StartLine ?? 0;
        public int EndLine   => Location?.EndLine   ?? 0;

        [CanBeNull]
        public Location Location { get; }

        /// <summary>
        /// Gibt den gesamte Bereich des Items an, oder null, falls es keinen definierten Bereich gibt (z.B. Projekt Items)
        /// </summary>
        [CanBeNull]
        public TextExtent? Extent => Location?.Extent;

        /// <summary>
        /// Gibt den Startpunkt des Bereichs an.
        /// </summary>
        public int Start => Extent?.Start ?? -1;

        /// <summary>
        /// Gibt den Endpunkt des Bereichs an.
        /// </summary>
        public int End => Extent?.End ?? -1;

        /// <summary>
        /// Gibt die Stelle an, an die bei Auswahl des Items hinnavigiert werden soll.
        /// </summary>
        public int NavigationPoint { get; }

        [NotNull]
        public ImmutableList<NavigationBarItem> Children { get; set; }

        public Microsoft.VisualStudio.TextManager.Interop.TextSpan LineSpan {
            get {
                return new Microsoft.VisualStudio.TextManager.Interop.TextSpan {
                    iStartIndex = 0,
                    iEndIndex   = 0,
                    iStartLine  = StartLine,
                    iEndLine    = EndLine,
                };
            }
        }

    }

}