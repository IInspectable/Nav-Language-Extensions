namespace Pharmatechnik.Nav.Language {

    sealed partial class InitNodeAliasSymbol : Symbol, IInitNodeAliasSymbol {

        // ReSharper disable once NotNullMemberIsNotInitialized Wird im Ctor der InitNode festgelegt
        public InitNodeAliasSymbol(string name, Location location) : base(name, location) {
        }

        public IInitNodeSymbol InitNode { get; set; }
    }
}