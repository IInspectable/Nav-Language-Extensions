#region Using Directives

using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TaskStatusCenter;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Utilities {

    [Export(typeof(TaskStatusProvider))]
    class TaskStatusProvider {

        readonly Lazy<IVsTaskStatusCenterService> _taskCenterService;

        [ImportingConstructor]
        public TaskStatusProvider(SVsServiceProvider serviceProvider) {
            _taskCenterService = new Lazy<IVsTaskStatusCenterService>(
                () => (IVsTaskStatusCenterService) serviceProvider.GetService(typeof(SVsTaskStatusCenterService)));
        }

        public TaskStatus CreateTaskStatus(string title) {

            var options = new TaskHandlerOptions {
                Title                  = title,
                ActionsAfterCompletion = CompletionActions.None
            };

            var taskCompletionSource = new TaskCompletionSource<bool>();

            var handler = _taskCenterService.Value?.PreRegister(options, data: default);
            handler?.RegisterTask(taskCompletionSource.Task);

            return new TaskStatus(taskCompletionSource, handler);
        }

    }

}