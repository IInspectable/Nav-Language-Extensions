#region Using Directives

using System;
using Microsoft.Build.Framework;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public class FileSpec {
        
        public FileSpec(string identity, string fileName) {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            FilePath = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public static FileSpec FromTaskItem(ITaskItem item) {
            return new FileSpec(item.ItemSpec, item.GetMetadata("FullPath"));
        }

        public string Identity { get; }
        public string FilePath { get; }
    }
}