#region Using Directives

using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Utilities.Logging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    partial class NavigationBar: TypeAndMemberDropdownBars {

        static readonly Logger Logger = Logger.Create<NavigationBar>();

        readonly NavLanguageService _languageService;
        readonly ModelBuilder       _modelBuilder;

        IntPtr _imageListHandle;

        public NavigationBar(NavLanguageService languageService, IVsTextView view): base(languageService) {
            _languageService = languageService;

            var componentModel = (IComponentModel) ServiceProvider.GetService(typeof(SComponentModel));

            IVsEditorAdaptersFactoryService editorAdapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            IWpfTextView                    textView                    = editorAdapterFactoryService.GetWpfTextView(view);
            ITextBuffer                     textBuffer                  = textView.TextBuffer;

            _modelBuilder = new ModelBuilder(this, textBuffer);

            textView.Caret.PositionChanged += OnCaretPositionChanged;
            VSColorTheme.ThemeChanged      += OnThemeChanged;

            UpdateImageList(synchronizeDropdowns: false);
        }

        private IServiceProvider ServiceProvider => _languageService.Package;

        void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            SynchronizeDropdowns();
        }

        void OnModelChanged() {
            SynchronizeDropdowns();
        }

        void OnThemeChanged(ThemeChangedEventArgs e) {

            ThreadHelper.JoinableTaskFactory.RunAsync(async () => {

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                UpdateImageList();
                SynchronizeDropdowns();
            });
        }

        public override int GetComboAttributes(int combo, out uint entries, out uint entryType, out IntPtr iList) {
            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            entryType = (uint) (DROPDOWNENTRYTYPE.ENTRY_TEXT | DROPDOWNENTRYTYPE.ENTRY_ATTR | DROPDOWNENTRYTYPE.ENTRY_IMAGE);
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
            iList   = _imageListHandle;
            entries = (uint) GetItems(combo).Count;

            return VSConstants.S_OK;
        }

        public override bool OnSynchronizeDropdowns(Microsoft.VisualStudio.Package.LanguageService languageService, IVsTextView textView, int line, int col,
                                                    ArrayList dropDownTypes,
                                                    ArrayList dropDownMembers,
                                                    ref int selectedType,
                                                    ref int selectedMember) {

            dropDownTypes.Clear();
            dropDownMembers.Clear();

            // Projekt Combobox
            var projectItems = GetItems(ProjectComboIndex);
            foreach (var entry in projectItems) {
                dropDownTypes.Add(new DropDownMember(entry.DisplayName, entry.ToSpan(), entry.ImageIndex, DROPDOWNFONTATTR.FONTATTR_PLAIN));
            }

            if (projectItems.Any()) {
                selectedType = 0;
            }

            // Task Entries
            var taskItems = GetItems(TaskComboIndex);
            foreach (var entry in taskItems) {
                dropDownMembers.Add(new DropDownMember(entry.DisplayName, entry.ToSpan(), entry.ImageIndex, DROPDOWNFONTATTR.FONTATTR_PLAIN));
            }

            selectedMember = CalculateActiveSelectionIndex(taskItems, line);

            return true;
        }

        /// <summary>
        /// Berechnet die zu wählende Selektion für die angegebene Combobox ausgehend von der aktuellen Caretposition
        /// </summary>
        int CalculateActiveSelectionIndex(ImmutableList<NavigationBarItem> items, int line) {

            var newIndex = -1;

            if (items.Any()) {

                var activeItem = items.FirstOrDefault(entry => line >= entry.StartLine && line <= entry.EndLine);

                if (activeItem != null) {
                    newIndex = items.IndexOf(activeItem);
                } else {
                    // Den ersten Eintrag nach dem Cursor wählen
                    var closestEntry = items.FirstOrDefault(entry => line < entry.StartLine && line < entry.EndLine);
                    if (closestEntry == null) {
                        // Den letzten Eintrag wählen
                        closestEntry = items.Last();
                    }

                    newIndex = items.IndexOf(closestEntry);
                }
            }

            return newIndex;
        }

        const int ProjectComboIndex = 0;
        const int TaskComboIndex    = 1;

        ImmutableList<NavigationBarItem> GetItems(int iCombo) {
            switch (iCombo) {
                case ProjectComboIndex:
                    return _modelBuilder.ProjectItems;
                case TaskComboIndex:
                    return _modelBuilder.TaskItems;
                default:
                    return ImmutableList<NavigationBarItem>.Empty;
            }
        }

        void UpdateImageList(bool synchronizeDropdowns = true) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var imageService            = (IVsImageService2) ServiceProvider.GetService(typeof(SVsImageService));
            var comboBoxBackgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxBackgroundColorKey);

            _imageListHandle = NavigationBarImages.GetImageList(comboBoxBackgroundColor, imageService);

            if (synchronizeDropdowns) {
                SynchronizeDropdowns();
            }
        }

        void SynchronizeDropdowns() {

            ThreadHelper.JoinableTaskFactory.RunAsync(async () => {

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                _languageService.SynchronizeDropdowns();
            });

        }

    }

}