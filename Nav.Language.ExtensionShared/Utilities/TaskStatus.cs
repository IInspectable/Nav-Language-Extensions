#region Using Directives

using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TaskStatusCenter;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Utilities {

    class TaskStatus: IDisposable {

        readonly TaskCompletionSource<bool> _taskCompletionSource;

        ITaskHandler _taskHandler;

        public TaskStatus(TaskCompletionSource<bool> taskCompletionSource,
                          ITaskHandler taskHandler) {
            _taskCompletionSource = taskCompletionSource;
            _taskHandler          = taskHandler;

        }

        public Task OnProgressChangedAsync(string message) {
            var data = new TaskProgressData {
                CanBeCanceled   = false,
                PercentComplete = null,
                ProgressText    = message
            };

            _taskHandler?.Progress.Report(data);

            return Task.CompletedTask;
        }

        public Task OnCompletedAsync() {
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose() {
            _taskCompletionSource.SetResult(true);
            _taskHandler = null;
        }

    }

}