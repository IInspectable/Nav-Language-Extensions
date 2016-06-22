namespace Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols {

    public enum LocationKind {
        Unspecified,

        TaskDefinition,
        InitDefinition,
        ExitDefinition,
        SignalTriggerDefinition,

        InitCallDeclaration,
        TaskExitDeclaration,
        TaskDeclaration,
        TriggerDeclaration,

        NodeDeclaration,
    }

}