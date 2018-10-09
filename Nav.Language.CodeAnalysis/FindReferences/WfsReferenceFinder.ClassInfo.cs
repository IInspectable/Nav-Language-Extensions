namespace Pharmatechnik.Nav.Language.CodeAnalysis.FindReferences {

    public partial class WfsReferenceFinder {

        struct ClassInfo {

            public ClassInfo(string projectName, string className) {
                ProjectName = projectName;
                ClassName   = className;
            }

            public string ProjectName { get; }
            public string ClassName   { get; }

        }

    }

}