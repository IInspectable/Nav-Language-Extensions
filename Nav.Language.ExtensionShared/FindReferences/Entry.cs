#region Using Directives

using System;
using System.Windows;

using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    abstract class Entry {

        protected Entry(FindReferencesPresenter presenter, DefinitionEntry definition) {
            Presenter  = presenter  ?? throw new ArgumentNullException(nameof(presenter));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public FindReferencesPresenter Presenter  { get; }
        public DefinitionEntry         Definition { get; }

        public abstract string Text { get; }

        public virtual object GetValue(string keyName) {
            switch (keyName) {
                case StandardTableKeyNames.Text:
                    // Wird für die Suche verwendet...
                    return Text;
                case StandardTableKeyNames2.Definition:
                    return Definition;
            }

            return null;
        }

        public virtual FrameworkElement TryCreateColumnContent() => null;

    }

}