namespace Pharmatechnik.Nav.Language.Extension.Commands {

    struct NavCommandState {

        public NavCommandState(bool isAvailable = false, bool isChecked = false, string displayText = null) {
            IsAvailable = isAvailable;
            IsChecked   = isChecked;
            DisplayText = displayText;
        }

        /// <summary>
        /// If true, the command should be visible and enabled in the UI.
        /// </summary>
        public bool IsAvailable { get; }

        /// <summary>
        /// If true, the command should appear as checked (i.e. toggled) in the UI.
        /// </summary>
        public bool IsChecked { get; }

        /// <summary>
        /// If specified, returns the custom text that should be displayed in the UI.
        /// </summary>
        public string DisplayText { get; }
        
        public static NavCommandState Available   => new NavCommandState(isAvailable: true);
        public static NavCommandState Unavailable => new NavCommandState(isAvailable: false);
    }
}