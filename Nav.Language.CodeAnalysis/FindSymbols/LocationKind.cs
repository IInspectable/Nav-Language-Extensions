namespace Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols {

    public enum LocationKind {
        Unspecified,

        TaskDefinition,
        InitDefinition,
        ExitDefinition,
        TriggerDefinition,

        InitCallDeclaration,
        TaskExitDeclaration,
        TaskDeclaration,
        TriggerDeclaration
    }

}