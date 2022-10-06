#region Using Directives

using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common; 

static class ContentTypeExtensions {

    public static bool MatchesAny(this IContentType dataContentType, IEnumerable<string> extensionContentTypes) {
        return extensionContentTypes.Any(dataContentType.IsOfType);
    }
}