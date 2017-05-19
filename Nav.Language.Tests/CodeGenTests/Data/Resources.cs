using System.IO;

namespace Nav.Language.Tests.CodeGenTests.Data {

    static class Resources {
        public static readonly string FrameworkStubsCode = LoadText("FrameworkStubs.cs");
        public static readonly string TaskANav = LoadText("TaskA.nav");

        static string LoadText(string resourceName) {

            var fullResourceName = $"{typeof(Resources).Namespace}.{resourceName}";

            using (Stream stream = typeof(Resources).Assembly.GetManifestResourceStream(fullResourceName))
                // ReSharper disable once AssignNullToNotNullAttribute Lass krachen...
            using (StreamReader reader = new StreamReader(stream)) {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
