#region Using Directives

using System.IO;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen.Templates {

    static class Resources {
        // ReSharper disable InconsistentNaming
        public static string IBeginWfsTemplate  => LoadText("IBeginWFS.stg");
        public static string IWfsTemplate       => LoadText("IWFS.stg");
        public static string WfsBaseTemplate    => LoadText("WFSBase.stg");
        public static string WFSOneShotTemplate => LoadText("WFSOneShot.stg");
        public static string CommonTemplate     => LoadText("Common.stg");
        // ReSharper restore InconsistentNaming

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