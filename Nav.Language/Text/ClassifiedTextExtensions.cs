#region Using Directives

using System.Linq;
using System.Text;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    public static class ClassifiedTextExtensions {

        public static string JoinText(this IEnumerable<ClassifiedText> parts) {
            return parts.Aggregate(new StringBuilder(), (sb, p) => sb.Append(p.Text), sb => sb.ToString());
        }

    }

}