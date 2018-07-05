namespace Pharmatechnik.Nav.Language {

    sealed partial class TaskNodeAliasSymbol: Symbol, ITaskNodeAliasSymbol {

        // ReSharper disable once NotNullMemberIsNotInitialized Wird im Builder festgelegt
        public TaskNodeAliasSymbol(string name, Location location): base(name, location) {
        }

        public ITaskNodeSymbol TaskNode { get; set; }

    }

}