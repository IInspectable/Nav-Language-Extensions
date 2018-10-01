#region Using Directives

using System;
using System.IO;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Utilities {

    struct ProjectEntry {

        public ProjectEntry(Uri directory, string name) {

            Name             = name      ?? throw new ArgumentNullException(nameof(name));
            ProjectDirectory = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        public string Name             { get; }
        public Uri    ProjectDirectory { get; }

    }

}