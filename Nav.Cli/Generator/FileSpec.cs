#region Using Directives

using System;
using System.IO;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.Generator {

    public class FileSpec {
        
        public FileSpec(string identity, string fileName) {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            FilePath = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public static FileSpec FromRelativePath(string baseDirectory, string file) {
            if (Path.IsPathRooted(file)) {
                var identity= PathHelper.GetRelativePath(baseDirectory, file);
                return new FileSpec(identity, file);
            }


            var path=  Path.GetFullPath(Path.Combine(baseDirectory, file));
            return new FileSpec(file, path);
        }
        
        public string Identity { get; }
        public string FilePath { get; }
    }
}