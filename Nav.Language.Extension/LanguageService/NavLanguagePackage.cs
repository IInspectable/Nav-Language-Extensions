#region Using Directives

using System;

using System.IO;
using System.Drawing;
using System.ComponentModel.Design;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.TextManager.Interop;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Utilities.Logging;

using Control = System.Windows.Controls.Control;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.LanguageService {

    #region Documentation
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    #endregion
    [ProvideLanguageService(typeof(NavLanguageInfo),
                            NavLanguageContentDefinitions.LanguageName,
                            101,
                            AutoOutlining         = true,        
                            MatchBraces           = true,
                            ShowSmartIndent       = false,
                            DefaultToInsertSpaces = true,
                            MatchBracesAtCaret    = true,
                            RequestStockColors    = true,       
                            ShowDropDownOptions   = false)]
    [InstalledProductRegistration("#110", "#112", ThisAssembly.ProductVersion, IconResourceID = 400)]
    [ProvideLanguageExtension(typeof(NavLanguageInfo), NavLanguageContentDefinitions.FileExtension)]
    [PackageRegistration(UseManagedResourcesOnly = true)]   
    [Guid(GuidList.NavPackageGuid)]
    [ProvideAutoLoad("{adfc4e64-0397-11d1-9f4e-00a0c911004f}")] // VSConstants.UICONTEXT_NoSolution
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")] // VSConstants.UICONTEXT_SolutionExists
    [ProvideShowBraceCompletion]
    [ProvideShowDropdownBarOption]
    sealed partial class NavLanguagePackage : Package {

        static readonly Logger Logger = Logger.Create<NavLanguagePackage>();

        // ReSharper disable once EmptyConstructor
        public NavLanguagePackage() {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            LoggerConfig.Initialize(Path.GetTempPath(), "Nav.Language.Extension");
        }

        #region Documentation
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        #endregion
        protected override void Initialize() {

            var langService = new NavLanguageInfo(this);
            ((IServiceContainer)this).AddService(langService.GetType(), langService, true);

            ((IServiceContainer)this).AddService(GetType(), this, true);

            base.Initialize();

            Logger.Info($"{nameof(NavLanguagePackage)}.{nameof(Initialize)}");
        }
        
        public static object GetGlobalService<TService>() where TService : class {
            return GetGlobalService(typeof(TService));
        }

        public static TInterface GetGlobalService<TService, TInterface>() where TInterface : class {
            return GetGlobalService(typeof(TService)) as TInterface;
        }

        static IServiceProvider GetServiceProvider() {
            var serviceProvider = GetGlobalService<NavLanguagePackage, IServiceProvider>();
            return serviceProvider;
        }

        public static VisualStudioWorkspace Workspace {
            get {
                var componentModel = GetGlobalService<SComponentModel, IComponentModel>();
                var workspace = componentModel.GetService<VisualStudioWorkspace>();
                return workspace;
            }
        }

        /// <summary>
        /// 1. Moves the caret to the specified index in the current snapshot.  
        /// 2. Updates the viewport so that the caret will be centered.
        /// 3. Moves focus to the text view to ensure the user can continue typing.
        /// </summary>
        public static void NavigateToLocation(ITextView textView, int location) {

            var bufferPosition = new SnapshotPoint(textView.TextBuffer.CurrentSnapshot, location);

            textView.Caret.MoveTo(bufferPosition);
            textView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(bufferPosition, 1), EnsureSpanVisibleOptions.AlwaysCenter);

            // ReSharper disable once SuspiciousTypeConversion.Global 
            (textView as Control)?.Focus();
        }

        [CanBeNull]
        public static IWpfTextView GoToLocationInPreviewTab(Location location) {

            using (Logger.LogBlock(nameof(GoToLocationInPreviewTab))) {

                if(location == null) {
                    return null;
                }

                IWpfTextView wpfTextView = null;
                if(location.FilePath != null) {
                    wpfTextView = OpenFileInPreviewTab(location.FilePath);
                }

                if(wpfTextView == null) {
                    return null;
                }

                if(location.Start == 0 && location.Length == 0) {
                    return wpfTextView;
                }

                var outliningManagerService = GetServiceProvider().GetMefService< IOutliningManagerService>();

                var snapshotSpan = location.ToSnapshotSpan(wpfTextView.TextSnapshot);
                if (wpfTextView.TryMoveCaretToAndEnsureVisible(snapshotSpan.Start, outliningManagerService)) {
                    wpfTextView.SetSelection(snapshotSpan);
                }

                return wpfTextView;
            }
        }

        [CanBeNull]
        public static IWpfTextView OpenFile(string file) {

            using(Logger.LogBlock(nameof(OpenFile))) {

                var serviceProvider = GetServiceProvider();

                Guid logicalView = Guid.Empty;
                VsShellUtilities.OpenDocument(serviceProvider, file, logicalView, out var _, out var _, out var windowFrame);

                return GetWpfTextViewFromFrame(windowFrame);
            }
        }
        
        [CanBeNull]
        public static IWpfTextView OpenFileInPreviewTab(string file) {

            ThreadHelper.ThrowIfNotOnUIThread();

            using(Logger.LogBlock(nameof(OpenFileInPreviewTab))) {

                IVsNewDocumentStateContext newDocumentStateContext = null;

                try {
                    var openDoc3 = GetGlobalService<SVsUIShellOpenDocument, IVsUIShellOpenDocument3>();

                    Guid reason = VSConstants.NewDocumentStateReason.Navigation;
                    newDocumentStateContext = openDoc3?.SetNewDocumentState((uint) __VSNEWDOCUMENTSTATE.NDS_Provisional, ref reason);

                    return OpenFile(file);

                } finally {
                    newDocumentStateContext?.Restore();
                }
            }
        }      

        [CanBeNull]
        public static ITextBuffer GetOpenTextBufferForFile(string filePath) {

            using(Logger.LogBlock(nameof(GetOpenTextBufferForFile))) {

                var package = GetGlobalService<NavLanguagePackage, NavLanguagePackage>();

                var componentModel = (IComponentModel) GetGlobalService(typeof(SComponentModel));
                var editorAdapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                if (VsShellUtilities.IsDocumentOpen(
                    package,
                    filePath,
                    Guid.Empty,
                    out IVsUIHierarchy _,
                    out uint _,
                    out IVsWindowFrame windowFrame)) {
                    IVsTextView view = VsShellUtilities.GetTextView(windowFrame);
                    if (view.GetBuffer(out var lines) == 0) {
                        if (lines is IVsTextBuffer buffer)
                            return editorAdapterFactoryService.GetDataBuffer(buffer);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the current IWpfTextView that is the active document.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public static IWpfTextView GetActiveTextView() {

            ThreadHelper.ThrowIfNotOnUIThread();

            using(Logger.LogBlock(nameof(GetActiveTextView))) {

                var monitorSelection = (IVsMonitorSelection) GetGlobalService(typeof(SVsShellMonitorSelection));
                if(monitorSelection == null) {
                    return null;
                }

                if(ErrorHandler.Failed(monitorSelection.GetCurrentElementValue((uint) VSConstants.VSSELELEMID.SEID_DocumentFrame, out var curDocument))) {
                    Logger.Error("Get VSConstants.VSSELELEMID.SEID_DocumentFrame failed");
                    return null;
                }
                if(!(curDocument is IVsWindowFrame frame)) {
                    Logger.Error($"{nameof(curDocument)} ist kein {nameof(IVsWindowFrame)}");
                    return null;
                }

                return GetWpfTextViewFromFrame(frame);
            }
        }

        [CanBeNull]
        static IWpfTextView GetWpfTextViewFromFrame(IVsWindowFrame frame) {

            ThreadHelper.ThrowIfNotOnUIThread();

            using(Logger.LogBlock(nameof(GetWpfTextViewFromFrame))) {
                if(ErrorHandler.Failed(frame.GetProperty((int) __VSFPROPID.VSFPROPID_DocView, out var docView))) {
                    Logger.Error("Get __VSFPROPID.VSFPROPID_DocView failed");
                    return null;
                }

                if(docView is IVsCodeWindow window) {
                    if(ErrorHandler.Failed(window.GetPrimaryView(out var textView))) {
                        Logger.Error("GetPrimaryView failed");
                        return null;
                    }

                    var model          = (IComponentModel) GetGlobalService(typeof(SComponentModel));
                    var adapterFactory = model.GetService<IVsEditorAdaptersFactoryService>();
                    var wpfTextView    = adapterFactory.GetWpfTextView(textView);
                    return wpfTextView;
                }
                Logger.Warn($"{nameof(GetWpfTextViewFromFrame)}: {nameof(docView)} ist kein {nameof(IVsCodeWindow)}");
                return null;
            }
        }

        public static _DTE DTE {
            get {
                _DTE dte = GetGlobalService<_DTE, _DTE>();
                return dte;
            }
        }

        public static BitmapSource GetBitmapSource(ImageMoniker moniker, Color? backgroundColor = null) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var imageAttributes = GetImageAttributes(_UIImageType.IT_Bitmap, _UIDataFormat.DF_WPF, backgroundColor);
            var imageService    = GetGlobalService<SVsImageService, IVsImageService2>();
            var result          = imageService?.GetImage(moniker, imageAttributes);

            object data =null;
            result?.get_Data(out data);
            return data as BitmapSource;
        }

        public static Bitmap GetBitmap(ImageMoniker moniker, Color? backgroundColor=null) {
            
            ThreadHelper.ThrowIfNotOnUIThread();

            var imageAttributes = GetImageAttributes(_UIImageType.IT_Bitmap, _UIDataFormat.DF_WinForms, backgroundColor);
            var imageService    = GetGlobalService<SVsImageService, IVsImageService2>();
            var result          = imageService?.GetImage(moniker, imageAttributes);

            object data = null;
            result?.get_Data(out data);
            return data as Bitmap;
        }

        public static IntPtr GetImageList(ImageMoniker moniker, Color? backgroundColor = null) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var imageAttributes = GetImageAttributes(_UIImageType.IT_ImageList, _UIDataFormat.DF_Win32, backgroundColor);
            var imageService    = GetGlobalService<SVsImageService, IVsImageService2>();
            var result          = imageService?.GetImage(moniker, imageAttributes);

            if(!(Microsoft.Internal.VisualStudio.PlatformUI.Utilities.GetObjectData(result) is IVsUIWin32ImageList imageListData)) {
                Logger.Warn($"{nameof(GetImageList)}: Unable to get IVsUIWin32ImageList");
                return IntPtr.Zero;
            }

            if(!ErrorHandler.Succeeded(imageListData.GetHIMAGELIST(out var imageListInt))) {
                Logger.Warn($"{nameof(GetImageList)}: Unable to get HIMAGELIST");
                return IntPtr.Zero;

            }
            return (IntPtr)imageListInt;            
        }

        static ImageAttributes GetImageAttributes(_UIImageType imageType, _UIDataFormat format, Color? backgroundColor, int width=16, int height=16) {

            ImageAttributes imageAttributes = new ImageAttributes {
                StructSize    = Marshal.SizeOf(typeof(ImageAttributes)),
                Dpi           = 96,
                Flags         = (uint)_ImageAttributesFlags.IAF_RequiredFlags,
                ImageType     = (uint)imageType,
                Format        = (uint)format,
                LogicalHeight = height,
                LogicalWidth  = width
            };
            if(backgroundColor.HasValue) {
                unchecked {
                    imageAttributes.Flags |= (uint)_ImageAttributesFlags.IAF_Background;
                }
                imageAttributes.Background = (uint)backgroundColor.Value.ToArgb();
            }
            return imageAttributes;
        }
    }
}
