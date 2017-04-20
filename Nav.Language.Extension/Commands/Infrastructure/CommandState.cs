namespace Pharmatechnik.Nav.Language.Extension.Commands {

    struct CommandState {

        public CommandState(bool isAvailable = false, bool isChecked = false, string displayText = null) {
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
        
        public static CommandState Available   => new CommandState(isAvailable: true);
        public static CommandState Unavailable => new CommandState(isAvailable: false);
    }
}