using System.IO;

namespace Nav.Language.Tests {

    static class Resources {
        public static readonly string FrameworkStubsCode = LoadText("FrameworkStubs.cs");
        public static readonly string TaskANav     = LoadText("TaskA.nav");
        public static readonly string AllRules     = LoadText("AllRules.nav");
        public static readonly string LargeNav     = LoadText("LargeNav.nav");
        public static readonly string NavWithError = LoadText("NavWithError.nav");
        //NavWithError
        static string LoadText(string resourceName) {

            var fullResourceName = $"{typeof(Resources).Namespace}.Resources.{resourceName}";

            using (Stream stream = typeof(Resources).Assembly.GetManifestResourceStream(fullResourceName))
                // ReSharper disable once AssignNullToNotNullAttribute Lass krachen...
            using (StreamReader reader = new StreamReader(stream)) {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
