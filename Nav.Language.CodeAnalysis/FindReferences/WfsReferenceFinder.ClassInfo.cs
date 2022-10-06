namespace Pharmatechnik.Nav.Language.CodeAnalysis.FindReferences; 

public static partial class WfsReferenceFinder {

    readonly struct ClassInfo {

        public ClassInfo(string projectName, string className) {
            ProjectName = projectName;
            ClassName   = className;
        }

        public string ProjectName { get; }
        public string ClassName   { get; }

    }

}