#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.Generator {

    public class FileSpec {
        
        public FileSpec(string identity, string fileName) {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            FilePath = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }
        
        public string Identity { get; }
        public string FilePath { get; }
    }
}