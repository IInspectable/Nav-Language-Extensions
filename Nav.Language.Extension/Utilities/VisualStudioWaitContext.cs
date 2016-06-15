#region Using Directives

using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Utilities {

    sealed partial class VisualStudioWaitContext : IWaitContext {

        const int DelayToShowDialogSecs = 1;

        readonly IVsThreadedWaitDialog3 _dialog;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly string _title;

        string _message;
        bool _allowCancel;

        public VisualStudioWaitContext(IVsThreadedWaitDialogFactory dialogFactory,
                                       string title,
                                       string message,
                                       bool allowCancel) {
            _title                   = title;
            _message                 = message;
            _allowCancel             = allowCancel;
            _cancellationTokenSource = new CancellationTokenSource();
            _dialog                  = CreateDialog(dialogFactory);
        }

        IVsThreadedWaitDialog3 CreateDialog(IVsThreadedWaitDialogFactory dialogFactory) {
            IVsThreadedWaitDialog2 dialog2;
            Marshal.ThrowExceptionForHR(dialogFactory.CreateInstance(out dialog2));
            //Contract.ThrowIfNull(dialog2);

            var dialog3 = (IVsThreadedWaitDialog3) dialog2;

            var callback = new Callback(this);

            dialog3.StartWaitDialogWithCallback(
                szWaitCaption     : _title,
                szWaitMessage     : _message,
                szProgressText    : null,
                varStatusBmpAnim  : null,
                szStatusBarText   : null,
                fIsCancelable     : _allowCancel,
                iDelayToShowDialog: DelayToShowDialogSecs,
                fShowProgress     : false,
                iTotalSteps       : 0,
                iCurrentStep      : 0,
                pCallback         : callback);

            return dialog3;
        }

        public CancellationToken CancellationToken {
            get {
                return _allowCancel
                    ? _cancellationTokenSource.Token
                    : CancellationToken.None;
            }
        }

        public string Message {
            get { return _message; }
            set {
                _message = value;
                UpdateDialog();
            }
        }

        public bool AllowCancel {
            get { return _allowCancel; }
            set {
                _allowCancel = value;
                UpdateDialog();
            }
        }

        void UpdateDialog() {
            bool hasCancelled;
            _dialog.UpdateProgress(
                szUpdatedWaitMessage: _message,
                szProgressText      : null,
                szStatusBarText     : null,
                iCurrentStep        : 0,
                iTotalSteps         : 0,
                fDisableCancel      : !_allowCancel,
                pfCanceled          : out hasCancelled);
        }

        public void UpdateProgress() {
        }

        public void Dispose() {
            int canceled;
            _dialog.EndWaitDialog(out canceled);
        }

        void OnCanceled() {
            if(_allowCancel) {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}