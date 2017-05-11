#region Using Directives

using System.IO;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen.Templates {

    static class Resources {
        
        // ReSharper disable once InconsistentNaming
        public static string IBeginWfsTemplate => LoadText("IBeginWFS.stg");

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