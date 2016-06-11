using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoToNav {

    class GoToNavTag : ITag {

        public GoToNavTag(NavTaskInfo navTaskInfo) {
            TaskInfo = navTaskInfo;

        }

        public NavTaskInfo TaskInfo { get; }
    }
}