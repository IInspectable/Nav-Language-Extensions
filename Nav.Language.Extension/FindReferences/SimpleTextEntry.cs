namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class SimpleTextEntry: Entry {

        SimpleTextEntry(FindReferencesPresenter presenter, DefinitionEntry definition, string text)
            : base(presenter, definition) {
            Text = text;
        }

        public static SimpleTextEntry Create(FindReferencesPresenter presenter, DefinitionEntry definition, string text) {
            return new SimpleTextEntry(presenter, definition, text);
        }

        public override string Text { get; }

    }

}