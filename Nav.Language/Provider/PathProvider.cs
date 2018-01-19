#region Using Directives

using System;
using System.IO;
using System.Linq;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class PathProvider: IPathProvider {

        /// <summary>
        /// generated
        /// </summary>
        public const string GeneratedFolderName = "generated";

        /// <summary>
        /// manual
        /// </summary>
        public const string LegacyManualFolderName = "manual";

        /// <summary>
        /// generated
        /// </summary>
        public const string GeneratedFileNameSuffix = "generated";

        /// <summary>
        /// cs
        /// </summary>
        public const string CSharpFileExtension = "cs";

        public PathProvider(string syntaxFileName, string taskName, string generateTo = null) {

            if (String.IsNullOrEmpty(syntaxFileName)) {
                throw new ArgumentException("Missing syntax filename", nameof(syntaxFileName));
            }

            if (String.IsNullOrEmpty(taskName)) {
                throw new ArgumentException("Missing taskName", nameof(taskName));
            }

            var syntaxFileDirectoryName = Path.GetDirectoryName(syntaxFileName);

            TaskName       = taskName;
            SyntaxFileName = syntaxFileName;

            IwflGeneratedDirectory = PathCombine(syntaxFileDirectoryName, CodeGenFacts.IwflNamespaceSuffix, generateTo, GeneratedFolderName);
            WflGeneratedDirectory  = PathCombine(syntaxFileDirectoryName, CodeGenFacts.WflNamespaceSuffix,  generateTo, GeneratedFolderName);
            WflDirectory           = PathCombine(syntaxFileDirectoryName, CodeGenFacts.WflNamespaceSuffix,  generateTo);
        }

        public string TaskName               { get; }
        public string WflDirectory           { get; }
        public string WflGeneratedDirectory  { get; }
        public string IwflGeneratedDirectory { get; }

        public virtual string SyntaxFileName    { get; }
        public virtual string WfsBaseFileName   => PathCombine(WflGeneratedDirectory,  $"{TaskName}{CodeGenFacts.WfsBaseClassSuffix}.{GeneratedFileNameSuffix}.{CSharpFileExtension}");
        public virtual string IWfsFileName      => PathCombine(IwflGeneratedDirectory, $"{CodeGenFacts.InterfacePrefix}{TaskName}{CodeGenFacts.WfsClassSuffix}.{GeneratedFileNameSuffix}.{CSharpFileExtension}");
        public virtual string IBeginWfsFileName => PathCombine(WflGeneratedDirectory,  $"{CodeGenFacts.BeginInterfacePrefix}{TaskName}{CodeGenFacts.WfsClassSuffix}.{GeneratedFileNameSuffix}.{CSharpFileExtension}");
        public virtual string WfsFileName       => PathCombine(WflDirectory,           $"{TaskName}{CodeGenFacts.WfsClassSuffix}.{CSharpFileExtension}");
        public virtual string LegacyWfsFileName => PathCombine(WflDirectory,           LegacyManualFolderName, $"{TaskName}{CodeGenFacts.WfsClassSuffix}.{CSharpFileExtension}");

        public string GetRelativePath(string fromPath, string toPath) {
            return PathHelper.GetRelativePath(fromPath, toPath);
        }

        public string GetToFileName(string toClassName) {
            return PathCombine(IwflGeneratedDirectory, $"{toClassName}.{GeneratedFileNameSuffix}.{CSharpFileExtension}");
        }

        static string PathCombine(string first, params string[] parts) {
            return parts.Where(part => !String.IsNullOrEmpty(part)).Aggregate(first, Path.Combine);
        }

    }

}