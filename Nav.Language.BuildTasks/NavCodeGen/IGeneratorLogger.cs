namespace Pharmatechnik.Nav.Language.BuildTasks {
    public interface IGeneratorLogger {
        void LogError(string message);
        void LogError(Diagnostic diag);
        void LogWarning(Diagnostic diag);
    }
}